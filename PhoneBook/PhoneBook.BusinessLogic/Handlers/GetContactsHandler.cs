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

    public void SetSortingParameters(string orderBy, bool isDescending)
    {
        _contactsRepository.OrderByProperty = orderBy;
        _contactsRepository.IsDescending = isDescending;
    }

    public void SetPagingParameters(int pageNumber, int pageSize)
    {
        _contactsRepository.PageNumber = pageNumber;
        _contactsRepository.PageSize = pageSize;
    }

    public async Task<PhoneBookPage> HandleAsync(string term)
    {
        return await _contactsRepository.GetContactsAsync(term);
    }
}
