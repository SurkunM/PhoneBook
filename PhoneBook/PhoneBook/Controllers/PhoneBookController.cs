using Microsoft.AspNetCore.Mvc;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Dto;
using PhoneBook.Model;

namespace PhoneBook.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhoneBookController : ControllerBase
{
    private readonly GetContactsHandler _getContactsHandler;

    private readonly CreateContactHandler _createContactHandler;

    public PhoneBookController(GetContactsHandler getContactsHandler, CreateContactHandler createContactHandler)
    {
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
        _createContactHandler = createContactHandler ?? throw new ArgumentNullException(nameof(createContactHandler));
    }

    public List<ContactDto> GetContacts()
    {
        return _getContactsHandler.Handle();
    }


    public void CreateContact([FromBody] ContactDto contact)
    {
        _createContactHandler.Handle(contact);
    }
}