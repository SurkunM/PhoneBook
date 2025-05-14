using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;
using PhoneBook.Model;

namespace PhoneBook.Contracts.IRepositories;

public interface IContactsRepository : IRepository<Contact>
{
    Task<PhoneBookPage> GetContactsAsync(GetContactsQueryParameters queryParameters);

    Task<List<ContactDto>> GetContactsAsync();

    Task<Contact?> FindContactByIdAsync(int id);

    Task DeleteRangeByIdAsync(List<int> rangeId);

    Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto);
}