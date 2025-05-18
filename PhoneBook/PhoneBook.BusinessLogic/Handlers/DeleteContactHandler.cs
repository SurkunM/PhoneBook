using Microsoft.Extensions.Logging;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class DeleteContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<DeleteContactHandler> _logger;

    public DeleteContactHandler(IUnitOfWork unitOfWork, ILogger<DeleteContactHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> DeleteSingleContactHandleAsync(int id)
    {
        try
        {
            var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();
            _unitOfWork.BeginTransaction();

            var contact = await contactsRepository.FindContactByIdAsync(id);

            if (contact is null)
            {
                return false;
            }

            contactsRepository.Delete(contact);

            await _unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction();

            _logger.LogError(ex, "Ошибка! При удалении контакта из БД произошла ошибка. Транзакция отменена");

            throw;
        }
    }

    public async Task DeleteAllSelectedContactHandleAsync(List<int> rangeId)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        await contactsRepository.DeleteRangeByIdAsync(rangeId);

        await _unitOfWork.SaveAsync();
    }
}