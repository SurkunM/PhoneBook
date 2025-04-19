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

    public PhoneBookController(
        GetContactsHandler getContactsHandler, CreateContactHandler createContactHandler,
        UpdateContactHandler updateContactHandler, DeleteContactHandler deleteContactHandler)
    {
        _createContactHandler = createContactHandler ?? throw new ArgumentNullException(nameof(createContactHandler));
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
        _updateContactHandler = updateContactHandler ?? throw new ArgumentNullException(nameof(updateContactHandler));
        _deleteContactHandler = deleteContactHandler ?? throw new ArgumentNullException(nameof(deleteContactHandler));
    }

    [HttpGet]
    public List<ContactDto> GetContacts()
    {
        return _getContactsHandler.Handle();
    }

    [HttpPost]
    public IActionResult CreateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isCreated = _createContactHandler.Handle(contactDto);

        if (!isCreated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult UpdateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isUpdated = _updateContactHandler.Handle(contactDto);

        if (!isUpdated)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }

        return Ok();
    }

    [HttpDelete]
    public IActionResult DeleteContact([FromBody] int id)
    {
        var contact = _deleteContactHandler.FindContactById(id);

        if (contact is null)
        {
            return BadRequest(ModelState);
        }

        var isDeleted = _deleteContactHandler.Handle(contact);

        if (!isDeleted)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера");
        }

        return Ok();
    }
}