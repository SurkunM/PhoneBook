using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;
using PhoneBook.Model;

namespace PhoneBook.Contracts.IRepositories;

public interface IContactsRepository : IRepository<Contact>
{
    Task<PhoneBookPage> GetContactsAsync(GetContactsQueryParameters queryParameters);

    Task<List<ContactDto>> GetContactsAsync();

    Task<Contact?> FindContactByIdAsync(int id);

    Task DeleteRangeByIdAsync(List<int> idsRange);

    Task<bool> IsPhoneExistAsync(ContactDto contactDto);

    Task<bool> CheckContactLimitAsync();
}