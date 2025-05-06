using PhoneBook.Contracts.Repositories;
using PhoneBook.Model;

namespace PhoneBook.BusinessLogic.Handlers;

public class DeleteContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public DeleteContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public Task<Contact?> FindContactByIdAsync(int id)
    {
        return _contactsRepository.FindContactByIdAsync(id);
    }

    public Task DeleteSingleContactHandlerAsync(Contact contact)
    {
        return _contactsRepository.DeleteAsync(contact);
    }

    public Task DeleteAllSelectedContactHandlerAsync(List<int> rangeId)
    {
        return _contactsRepository.DeleteRangeByIdAsync(rangeId);
    }
}