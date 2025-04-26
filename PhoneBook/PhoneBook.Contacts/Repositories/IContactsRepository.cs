using PhoneBook.Contracts.Dto;
using PhoneBook.Model;

namespace PhoneBook.Contracts.Repositories;

public interface IContactsRepository : IRepository<Contact>
{
    Task<List<ContactDto>> GetContactsAsync(string term);

    Task<Contact?> FindContactByIdAsync(int id);

    Task<bool> DeleteRangeByIdAsync(List<int> rangeId);

    Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto);
}