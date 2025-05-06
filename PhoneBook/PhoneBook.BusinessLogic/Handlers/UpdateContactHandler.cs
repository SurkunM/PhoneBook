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

    public Task HandlerAsync(ContactDto contactDto)
    {
        return _contactsRepository.UpdateAsync(contactDto.ToModel());
    }

    public Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        return _contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}