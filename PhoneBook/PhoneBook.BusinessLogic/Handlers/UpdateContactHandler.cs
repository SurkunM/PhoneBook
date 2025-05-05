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

    public async Task<bool> HandlerAsync(ContactDto contactDto)
    {
        await _contactsRepository.UpdateAsync(contactDto.ToModel());

        return true;
    }

    public async Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        return await _contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}