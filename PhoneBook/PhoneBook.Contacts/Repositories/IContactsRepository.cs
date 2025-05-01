using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;
using PhoneBook.Model;

namespace PhoneBook.Contracts.Repositories;

public interface IContactsRepository : IRepository<Contact>
{
    Task<PhoneBookPage> GetContactsAsync(string term);

    Task<Contact?> FindContactByIdAsync(int id);

    Task<bool> DeleteRangeByIdAsync(List<int> rangeId);

    Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto);

    void SetSortingParameters(string orderBy, bool isDescending);

    void SetPagingParameters(int pageNumber, int pageSize);

    void SetRepositoryDefaultState();
}