using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class DeleteContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public DeleteContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public bool DeleteSingleContactHandle(int id)
    {
        var contact = _contactsRepository.FindContactById(id);

        if (contact is null)
        {
            return false;
        }

        _contactsRepository.Delete(contact);

        return true;
    }

    public bool DeleteAllSelectedContactHandle(List<int> rangeId)
    {
        _contactsRepository.DeleteRangeById(rangeId);

        return true;
    }
}