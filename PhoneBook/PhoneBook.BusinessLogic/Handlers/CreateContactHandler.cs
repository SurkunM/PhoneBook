using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class CreateContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public CreateContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public Task HandlerAsync(ContactDto contactDto)
    {
        return _contactsRepository.CreateAsync(contactDto.ToModel());
    }

    public Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        return _contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}
