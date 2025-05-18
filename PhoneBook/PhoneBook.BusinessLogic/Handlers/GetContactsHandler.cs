using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;
using PhoneBook.Contracts.Responses;

namespace PhoneBook.BusinessLogic.Handlers;

public class GetContactsHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContactsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public Task<PhoneBookPage> HandleAsync(GetContactsQueryParameters queryParameters)
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        return contactsRepository.GetContactsAsync(queryParameters);
    }
}
