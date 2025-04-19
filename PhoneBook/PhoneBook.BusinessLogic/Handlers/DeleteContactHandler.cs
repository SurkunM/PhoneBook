using PhoneBook.Contracts.Dto;
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

    public bool Handle(Contact contact)
    {
        _contactsRepository.Delete(contact);
        _contactsRepository.Save();

        return true;
    }

    public Contact? FindContactById(int id)
    {
        return _contactsRepository.FindContactById(id);
    }
}