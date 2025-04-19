using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    public ContactsRepository(PhoneBookDbContext dbContext) : base(dbContext) { }

    public List<ContactDto> GetContacts()
    {
        return _dbSet
            .AsNoTracking()
            .Select(c => c.ToDto())
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)            
            .ToList();
    }

    public Contact? FindContactById(int id)
    {
        return _dbSet.FirstOrDefault(c => c.Id == id);
    }
}
