using PhoneBook.Contracts.Dto;
using PhoneBook.Model;

namespace PhoneBook.Contracts.Repositories;

public interface IContactsRepository : IRepository<Contact>
{
    List<ContactDto> GetContacts(string term);

    bool DeleteRangeById(List<int> rangeId);

    Contact? FindContactById(int id);
}