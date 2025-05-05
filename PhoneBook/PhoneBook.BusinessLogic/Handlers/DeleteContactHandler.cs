using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class DeleteContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public DeleteContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public async Task<bool> DeleteSingleContactHandlerAsync(int id)
    {
        var contact = await _contactsRepository.FindContactByIdAsync(id);

        if (contact is null)
        {
            return false;
        }

        await _contactsRepository.DeleteAsync(contact);

        return true;
    }

    public async Task<bool> DeleteAllSelectedContactHandlerAsync(List<int> rangeId)
    {
        await _contactsRepository.DeleteRangeByIdAsync(rangeId);

        return true;
    }
}