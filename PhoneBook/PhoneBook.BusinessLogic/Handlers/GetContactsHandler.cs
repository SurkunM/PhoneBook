using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;

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

    public async Task<List<ContactDto>> HandleAsync(string term)
    {
        return await _contactsRepository.GetContactsAsync(term);
    }
}
