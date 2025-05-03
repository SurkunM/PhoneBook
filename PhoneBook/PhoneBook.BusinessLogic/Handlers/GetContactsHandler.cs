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

    public void SetSortingParameters(string orderBy, bool isDescending)
    {
        _contactsRepository.SetSortingParameters(orderBy, isDescending);
    }

    public void SetPagingParameters(int pageNumber, int pageSize)
    {
        _contactsRepository.SetPagingParameters(pageNumber, pageSize);
    }

    public async Task<PhoneBookPage> HandleAsync(string term)
    {
        return await _contactsRepository.GetContactsAsync(term);
    }
}
