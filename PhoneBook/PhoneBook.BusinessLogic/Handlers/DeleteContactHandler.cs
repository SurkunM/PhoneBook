using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class DeleteContactHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> DeleteSingleContactHandlerAsync(int id)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();
        var contact = await contactsRepository.FindContactByIdAsync(id);

        if (contact is null)
        {
            return false;
        }

        contactsRepository.Delete(contact);

        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task DeleteAllSelectedContactHandlerAsync(List<int> rangeId)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        await contactsRepository.DeleteRangeByIdAsync(rangeId);

        await _unitOfWork.SaveAsync();
    }
}