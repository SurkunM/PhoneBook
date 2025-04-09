using Microsoft.AspNetCore.Mvc;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Dto;

namespace PhoneBook.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhoneBookController : ControllerBase //TODO: 1:14:50
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
