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
    public ActionResult<List<ContactDto>> GetContacts([FromQuery] string term = "")
    {
        if (term is null)
        {
            _logger.LogError("При создании контакта произошла ошибка.");

            return BadRequest("Передано значение null.");
        }

        return _getContactsHandler.Handle(term.ToUpper().Trim());
    }

    [HttpPost]
    public IActionResult CreateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Переданы не корректные данные контакта.");
            return BadRequest(ModelState);
        }

        if (_createContactHandler.CheckIsPhoneExist(contactDto.Phone))
        {
            _logger.LogError("Контакт с таким номером уже существует.");

            return BadRequest("Контакт с таким номером уже существует.");
        }

        try
        {
            _logger.LogInformation("Создание контакта.");
            _createContactHandler.Handle(contactDto);

            return Ok();
        }
        catch (Exception)
        {
            _logger.LogError("При создании контакта произошла ошибка.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public IActionResult UpdateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("При редактировании контакта произошла ошибка. Не корректно заполнены поля данных.");
            return BadRequest(ModelState);
        }

        if (_updateContactHandler.CheckIsPhoneExist(contactDto.Phone))
        {
            _logger.LogError("При редактировании контакта произошла ошибка. Контакт с таким номером уже существует.");

            return BadRequest("Контакт с таким номером уже существует.");
        }

        try
        {
            _updateContactHandler.Handle(contactDto);

            return Ok();
        }
        catch
        {
            _logger.LogError("При создании контакта произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public IActionResult DeleteContact([FromBody] int id)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("При удалении контакта произошла ошибка.");

            return BadRequest(ModelState);
        }

        try
        {
            _deleteContactHandler.DeleteSingleContactHandle(id);

            return Ok();
        }
        catch
        {
            _logger.LogError("При удалении контакта произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public IActionResult DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            _logger.LogError("Не переданы данные для удаления.");

            return BadRequest("Не переданы данные для удаления.");
        }

        try
        {
            _deleteContactHandler.DeleteAllSelectedContactHandle(selectedContactsId);

            return Ok();
        }
        catch
        {
            _logger.LogError("При удалении выбранных контактов произошла ошибка.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }
}