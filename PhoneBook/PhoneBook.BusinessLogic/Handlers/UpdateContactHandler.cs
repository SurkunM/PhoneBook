using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class UpdateContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public UpdateContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public bool Handle(ContactDto contactDto)
    {
        _contactsRepository.Update(contactDto.ToModel());
        _contactsRepository.Save();

        return true;
    }
}