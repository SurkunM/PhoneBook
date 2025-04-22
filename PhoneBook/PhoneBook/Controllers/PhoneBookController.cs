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
    public List<ContactDto> GetContacts()//TODO: Сделать логирование!
    {
        return _getContactsHandler.Handle();
    }

    [HttpPost]
    public IActionResult CreateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Переданы не корректные данные контакта.");
            return BadRequest(ModelState);
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
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }
    }

    [HttpPost]
    public IActionResult UpdateContact(ContactDto contactDto)//TODO: 3.Сейчас нет проверки что номер уже существует! Разобраться с валидацией
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _updateContactHandler.Handle(contactDto);

            return Ok();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }
    }

    [HttpDelete]
    public IActionResult DeleteContact([FromBody] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _deleteContactHandler.DeleteSingleContactHandle(id);

            return Ok();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }
    }

    [HttpDelete]
    public IActionResult DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            return BadRequest("Не переданы данные для удаления");
        }

        try
        {
            _deleteContactHandler.DeleteAllSelectedContactHandle(selectedContactsId);

            return Ok();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }
    }
}