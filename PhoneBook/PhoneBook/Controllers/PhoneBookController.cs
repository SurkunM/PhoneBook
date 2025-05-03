using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;

namespace PhoneBook.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhoneBookController : ControllerBase
{
    private readonly CreateContactHandler _createContactHandler;

    private readonly GetContactsHandler _getContactsHandler;

    private readonly UpdateContactHandler _updateContactHandler;

    private readonly DeleteContactHandler _deleteContactHandler;

    private readonly ILogger<PhoneBookController> _logger;

    private readonly IConfiguration _configuration;

    public PhoneBookController(
        GetContactsHandler getContactsHandler, CreateContactHandler createContactHandler,
        UpdateContactHandler updateContactHandler, DeleteContactHandler deleteContactHandler, ILogger<PhoneBookController> logger, IConfiguration configuration)
    {
        _createContactHandler = createContactHandler ?? throw new ArgumentNullException(nameof(createContactHandler));
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
        _updateContactHandler = updateContactHandler ?? throw new ArgumentNullException(nameof(updateContactHandler));
        _deleteContactHandler = deleteContactHandler ?? throw new ArgumentNullException(nameof(deleteContactHandler));
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<PhoneBookPage>> GetContacts([FromQuery] GetContactsQueryParameters queryParameters)
    {
        //if (term is null || sortBy is null)
        //{
        //    _logger.LogError("Ошибка! При запросе на получение контакта передано значение null. " + ЭТО СДЕЛАТ ЧЕРЕЗ АТРИБУТЫ PARAMETRES
        //        "Term: {Term}, SortBy: {SortBy}", term, sortBy);

        //    return BadRequest("Передано значение null.");
        //}

        //if (pageNumber < 1 || pageSize < 1)
        //{
        //    _logger.LogError("Ошибка! При запросе на получение контакта передано не корректное значение номера или размера страницы. " +
        //        "PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);

        //    return BadRequest("Передано не корректное значение номера или размера страницы.");
        //}

        try
        {
            var contacts = await _getContactsHandler.HandleAsync(queryParameters);

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение контактов не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        if (contactDto is null)
        {
            _logger.LogError("Ошибка! Объект ContactDto пуст.");

            return BadRequest("Объект \"Новый контакт\" пуст.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания контакта. {ContactDto}", contactDto);

            return UnprocessableEntity(ModelState);
        }

        if (await _createContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Ошибка! Попытка создать контакт с существующим в бд номером. {Phone}", contactDto.Phone);

            return Conflict("Контакт с таким номером уже существует.");
        }

        try
        {
            await _createContactHandler.HandleAsync(contactDto);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Контакт не создан.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(ContactDto contactDto)
    {
        if (contactDto is null)
        {
            _logger.LogError("Ошибка! Объект ContactDto пуст.");

            return BadRequest("Объект ContactDto пуст.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Не корректно заполнены поля для изменения контакта. {ContactDto}", contactDto);

            return UnprocessableEntity(ModelState);
        }

        if (await _updateContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Ошибка! Попытка добавить номер телефона, который уже существует. {Phone}", contactDto.Phone);

            return Conflict("Номер уже существует.");
        }

        try
        {
            await _updateContactHandler.HandleAsync(contactDto);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Контакт не изменен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContact([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("Передано значение id меньше нуля. id={id}", id);

            BadRequest("Передано не корректное значение.");
        }

        try
        {
            var isDelete = await _deleteContactHandler.DeleteSingleContactHandleAsync(id);

            if (!isDelete)
            {
                _logger.LogError("Ошибка! Контакт для удаления не найден. id={id}", id);

                return BadRequest("Контакт для удаления не найден.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление контакта не выполнено. id={id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            _logger.LogError("Ошибка! Не переданы данные для удаления.{ContactsId}", selectedContactsId);

            return BadRequest("Не переданы данные для удаления.");
        }

        try
        {
            await _deleteContactHandler.DeleteAllSelectedContactHandleAsync(selectedContactsId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление выбранных контактов не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    private void ConvertToExcelTable(List<ContactDto> personsList)
    {
        ArgumentNullException.ThrowIfNull(personsList);

        using var workbook = new XLWorkbook();

        var worksheet = workbook.AddWorksheet();
        worksheet.Range("A1:D1").Merge();

        worksheet.Cell("A1").SetValue("Сотрудники");
        worksheet.Cell("A1").Style.Font.FontSize = 12;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var table = worksheet.Cell("A2").InsertTable(personsList, "Сотрудники");

        table.Field("LastName").Name = "Фамилия";
        table.Field("FirstName").Name = "Имя";
        table.Field("Age").Name = "Возраст";
        table.Field("Phone").Name = "Телефон";

        table.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        table.ShowAutoFilter = false;
        table.Theme = XLTableTheme.TableStyleMedium5;

        worksheet.Columns().AdjustToContents();

        // 3. Сохраняем файл по пути из конфига
        var exportPath = _configuration["ExcelExport:Path"];
        var fileName = $"contacts_export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var fullPath = Path.Combine(exportPath, fileName);

        Directory.CreateDirectory(exportPath);
        workbook.SaveAs(fullPath);
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel()
    {
        return Ok();

        // 1. Получаем данные из БД
        var contacts = await _getContactsHandler.AllContactsHandleAsync();

        ConvertToExcelTable(contacts);

        // 2. Создаем Excel-файл (пример с EPPlus)
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Contacts");

        // Заполняем заголовки (первую строку)
        var headerRow = worksheet.FirstRow();
        headerRow.Style.Font.Bold = true;

        // Если хотите автоматически создать заголовки из свойств модели:
        worksheet.Cell(1, 1).InsertTable(contacts, true);

        // ИЛИ вручную (пример для кастомных заголовков):
        // worksheet.Cell(1, 1).Value = "ID";
        // worksheet.Cell(1, 2).Value = "Name";
        // ...
        // Затем заполняем данные:
        // for (int i = 0; i < contacts.Count; i++)
        // {
        //     worksheet.Cell(i + 2, 1).Value = contacts[i].Id;
        //     worksheet.Cell(i + 2, 2).Value = contacts[i].Name;
        //     ...
        // }

        // 3. Сохраняем файл по пути из конфига
        var exportPath = _configuration["ExcelExport:Path"];
        var fileName = $"contacts_export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var fullPath = Path.Combine(exportPath, fileName);

        Directory.CreateDirectory(exportPath);
        workbook.SaveAs(fullPath);

        // 4. Возвращаем файл клиенту
        var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        memoryStream.Position = 0;

        return File(memoryStream,
                  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                  fileName);
    }
}