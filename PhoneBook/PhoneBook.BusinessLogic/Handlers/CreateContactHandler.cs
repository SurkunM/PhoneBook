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

    public async Task<bool> HandleAsync(ContactDto contactDto)
    {
        await _contactsRepository.CreateAsync(contactDto.ToModel());

        return true;
    }

    public async Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        return await _contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}
