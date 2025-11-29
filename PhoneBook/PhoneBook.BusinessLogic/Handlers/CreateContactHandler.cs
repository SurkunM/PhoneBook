using Microsoft.Extensions.Logging;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Exceptions;
using PhoneBook.Contracts.Extensions;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class CreateContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateContactHandler> _logger;

    public CreateContactHandler(IUnitOfWork unitOfWork, ILogger<CreateContactHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ContactDto contactDto)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        try
        {
            _unitOfWork.BeginTransaction();

            if (!await contactsRepository.CheckContactLimitAsync())
            {
                throw new ContactsLimitException("Превышен лимит контактов");
            }

            if (await contactsRepository.IsPhoneExistAsync(contactDto))
            {
                throw new DuplicatePhoneException("Контакт с таким номером уже существует");
            }

            await contactsRepository.CreateAsync(contactDto.ToModel());

            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "При создании контакта произошла ошибка. Транзакция отменена.");

            _unitOfWork.RollbackTransaction();

            throw;
        }
    }
}
