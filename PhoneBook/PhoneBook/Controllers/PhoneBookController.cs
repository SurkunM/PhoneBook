using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
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

    public PhoneBookController(
        GetContactsHandler getContactsHandler, CreateContactHandler createContactHandler,
        UpdateContactHandler updateContactHandler, DeleteContactHandler deleteContactHandler, ILogger<PhoneBookController> logger)
    {
        _createContactHandler = createContactHandler ?? throw new ArgumentNullException(nameof(createContactHandler));
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
        _updateContactHandler = updateContactHandler ?? throw new ArgumentNullException(nameof(updateContactHandler));
        _deleteContactHandler = deleteContactHandler ?? throw new ArgumentNullException(nameof(deleteContactHandler));
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PhoneBookPage>> GetContacts([FromQuery] GetContactsQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение контакта переданы не корректные параметры страницы. ");

            return BadRequest(ModelState);
        }

        try
        {
            var contacts = await _getContactsHandler.HandlerAsync(queryParameters);

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
            await _createContactHandler.HandlerAsync(contactDto);

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
            await _updateContactHandler.HandlerAsync(contactDto);

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
            var isDelete = await _deleteContactHandler.DeleteSingleContactHandlerAsync(id);

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
            await _deleteContactHandler.DeleteAllSelectedContactHandlerAsync(selectedContactsId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление выбранных контактов не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel()//TODO: Нужно сделать для этого отдельный класс и хандлер
    {
        try
        {
            var contactsDto = await _getContactsHandler.AllContactsHandlerAsync();

            if (contactsDto is null || contactsDto.Count == 0)
            {
                return BadRequest("Нет данных для экспорта");
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet();

            worksheet.Range("A1:С1").Merge();
            worksheet.Cell("A1").SetValue("Контакты");
            worksheet.Cell("A1").Style.Font.FontSize = 12;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var contacts = contactsDto
                 .Select(c => new
                 {
                     c.FirstName,
                     c.LastName,
                     c.Phone
                 })
                 .ToList();

            var table = worksheet.Cell("A2").InsertTable(contacts, "Контакты");

            table.Field("LastName").Name = "Фамилия";
            table.Field("FirstName").Name = "Имя";
            table.Field("Phone").Name = "Телефон";

            table.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            table.ShowAutoFilter = false;
            table.Theme = XLTableTheme.TableStyleMedium5;

            worksheet.Columns().AdjustToContents();

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);

            memoryStream.Position = 0;

            var excel = File(
                fileStream: memoryStream,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: $"contacts_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );

            return excel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при экспорте в Excel");

            return StatusCode(500, "Произошла ошибка при генерации файла");
        }
    }
}