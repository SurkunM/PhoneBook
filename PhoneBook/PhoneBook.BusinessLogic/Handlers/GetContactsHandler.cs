using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.Contracts.Responses;
using System.Threading.Tasks;

namespace PhoneBook.BusinessLogic.Handlers;

public class GetContactsHandler
{
    private readonly IContactsRepository _contactsRepository;

    public GetContactsHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public async Task<PhoneBookPage> HandlerAsync(GetContactsQueryParameters queryParameters)
    {
        return await _contactsRepository.GetContactsAsync(queryParameters);
    }

    public async Task<List<ContactDto>> AllContactsHandlerAsync()
    {
        return await _contactsRepository.GetContactsAsync();
    }
}
