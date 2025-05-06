using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.Contracts.Responses;

namespace PhoneBook.BusinessLogic.Handlers;

public class GetContactsHandler
{
    private readonly IContactsRepository _contactsRepository;

    public GetContactsHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public Task<PhoneBookPage> HandlerAsync(GetContactsQueryParameters queryParameters)
    {
        return _contactsRepository.GetContactsAsync(queryParameters);
    }
}
