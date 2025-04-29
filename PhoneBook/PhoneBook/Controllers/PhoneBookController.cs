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
    public async Task<ActionResult<PhoneBookPage>> GetContacts(
            [FromQuery] string term = "",
            [FromQuery] string sortBy = "lastName",
            [FromQuery] bool isDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
    {
        if (term is null || sortBy is null)
        {
            _logger.LogError("Ошибка! При запросе на получение контакта передано значение null");

            return BadRequest("Передано значение null.");
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            _logger.LogError("Ошибка! При запросе на получение контакта передано не корректное значение номера или размера страницы.");

            return BadRequest("Передано не корректное значение номера или размера страницы.");
        }

        try
        {
            _getContactsHandler.SetPagingParameters(pageNumber, pageSize);
            _getContactsHandler.SetSortingParameters(sortBy, isDescending);

            var contacts = await _getContactsHandler.HandleAsync(term);

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
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания контакта.");

            return UnprocessableEntity(ModelState);
        }

        if (await _createContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Ошибка! Попытка создать контакт с существующим в бд номером.");

            return Conflict("Контакт с таким номером уже существует.");
        }

        try
        {
            _logger.LogInformation("Создание контакта.");//?

            await _createContactHandler.HandleAsync(contactDto);

            return Ok();
        }
        catch (Exception)
        {
            _logger.LogError("Ошибка! Контакт не создан.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Не корректно заполнены поля для изменения контакта.");

            return UnprocessableEntity(ModelState);
        }

        if (await _updateContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Ошибка! Попытка добавить номер телефона, который уже существует.");

            return Conflict("Номер уже существует.");
        }

        try
        {
            await _updateContactHandler.HandleAsync(contactDto);

            return Ok();
        }
        catch
        {
            _logger.LogError("Ошибка! Контакт не изменен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContact([FromBody] int id)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При удалении контакта произошла ошибка.");//Сюда проверку сущ. id

            return BadRequest(ModelState);
        }

        try
        {
            var isDelete = await _deleteContactHandler.DeleteSingleContactHandleAsync(id);//сделать отдельно 

            if (!isDelete)
            {
                return BadRequest("Контакт для удаления не найден.");

            }

            return Ok();
        }
        catch
        {
            _logger.LogError("Ошибка! Удаление контакта не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            _logger.LogError("Ошибка! Не переданы данные для удаления.");

            return BadRequest("Не переданы данные для удаления.");
        }

        try
        {
            await _deleteContactHandler.DeleteAllSelectedContactHandleAsync(selectedContactsId);

            return Ok();
        }
        catch
        {
            _logger.LogError("Ошибка! Удаление выбранных контактов не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}