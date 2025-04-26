using Microsoft.AspNetCore.Mvc;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Dto;

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
    public async Task<ActionResult<List<ContactDto>>> GetContacts([FromQuery] string term = "")
    {
        if (term is null)
        {
            _logger.LogError("При создании контакта произошла ошибка.");

            return BadRequest("Передано значение null.");
        }

        try
        {
            var contacts = await _getContactsHandler.HandleAsync(term);

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении контактов");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Переданы не корректные данные контакта.");

            return UnprocessableEntity(ModelState);
        }

        if (await _createContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Контакт с таким номером уже существует.");

            return Conflict("Контакт с таким номером уже существует.");
        }

        try
        {
            _logger.LogInformation("Создание контакта.");
            await _createContactHandler.HandleAsync(contactDto);

            return Ok();
        }
        catch (Exception)
        {
            _logger.LogError("При создании контакта произошла ошибка.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Не корректно заполнены поля данных.");
            return UnprocessableEntity(ModelState);
        }

        if (await _updateContactHandler.CheckIsPhoneExistAsync(contactDto))
        {
            _logger.LogError("Попытка добавить номер телефона, который уже существует.");

            return Conflict("Номер уже существует.");
        }

        try
        {
            await _updateContactHandler.HandleAsync(contactDto);

            return Ok();
        }
        catch
        {
            _logger.LogError("При создании контакта произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContact([FromBody] int id)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("При удалении контакта произошла ошибка.");

            return BadRequest(ModelState);
        }

        try
        {
            var isDelete = await _deleteContactHandler.DeleteSingleContactHandleAsync(id);

            if (!isDelete)
            {
                return BadRequest("Контакт для удаления не найден.");

            }

            return Ok();
        }
        catch
        {
            _logger.LogError("При удалении контакта произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            _logger.LogError("Не переданы данные для удаления.");

            return BadRequest("Не переданы данные для удаления.");
        }

        try
        {
            await _deleteContactHandler.DeleteAllSelectedContactHandleAsync(selectedContactsId);

            return Ok();
        }
        catch
        {
            _logger.LogError("При удалении выбранных контактов произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}