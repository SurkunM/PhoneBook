using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class CreateContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task HandleAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();
        await contactsRepository.CreateAsync(contactDto.ToModel());

        await _unitOfWork.SaveAsync();
    }

    public Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        return contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}
