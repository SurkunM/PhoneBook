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
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                PhoneNumbers = c.PhoneNumbers
                    .Select(p => new PhoneNumberDto
                    {
                        Id = p.Id,
                        Phone = p.Phone,
                        Type = p.Type
                    })
                    .OrderBy(p => p.Phone)
                    .ToList()
            })
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ThenBy(c => c.MiddleName)
            .ToList();
    }
}
