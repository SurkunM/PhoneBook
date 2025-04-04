using Microsoft.AspNetCore.Mvc;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contacts.Dto;

namespace PhoneBook.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhoneBookController : ControllerBase
{
    private readonly GetContactsHandler _getContactsHandler;

    public PhoneBookController(GetContactsHandler getContactsHandler)
    {
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
    }

    public List<ContactDto> GetContacts()
    {
        return _getContactsHandler.Handle();
    }
}
