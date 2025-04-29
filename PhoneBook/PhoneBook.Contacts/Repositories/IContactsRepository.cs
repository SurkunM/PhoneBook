using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;
using PhoneBook.Model;

namespace PhoneBook.Contracts.Repositories;

public interface IContactsRepository : IRepository<Contact>
{
    int PageNumber { get; set; }

    int PageSize { get; set; }

    bool IsDescending { get; set; }

    string OrderByProperty { get; set; }

    Task<PhoneBookPage> GetContactsAsync(string term);

    Task<Contact?> FindContactByIdAsync(int id);

    Task<bool> DeleteRangeByIdAsync(List<int> rangeId);

    Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto);
}