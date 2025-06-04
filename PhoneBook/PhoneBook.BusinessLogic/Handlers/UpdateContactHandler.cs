using Microsoft.Extensions.Logging;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Extensions;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class UpdateContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UpdateContactHandler> _logger;

    public UpdateContactHandler(IUnitOfWork unitOfWork, ILogger<UpdateContactHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HandleAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            if (contactsRepository.IsPhoneExist(contactDto))
            {
                _logger.LogError("Ошибка! Попытка добавить номер телефона, который уже существует. {Phone}", contactDto.Phone);

                return false;
            }

            contactsRepository.Update(contactDto.ToModel());

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "При обновлении данных контакта произошла ошибка. Транзакция отменена.");

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}