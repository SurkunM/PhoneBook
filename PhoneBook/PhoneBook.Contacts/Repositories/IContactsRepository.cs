using PhoneBook.Contracts.Dto;
using PhoneBook.Model;

namespace PhoneBook.Contracts.Repositories;

public interface IContactsRepository : IRepository<Contact>
{
    List<ContactDto> GetContacts();

}
