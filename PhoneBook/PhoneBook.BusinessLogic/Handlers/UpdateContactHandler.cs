using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class UpdateContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task HandlerAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        contactsRepository.Update(contactDto.ToModel());

        return _unitOfWork.SaveAsync();
    }

    public Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        return contactsRepository.CheckIsPhoneExistAsync(contactDto);
    }
}