using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    public ContactsRepository(PhoneBookDbContext dbContext) : base(dbContext) { }

    public List<ContactDto> GetContacts(string term)
    {
        if (term.Length == 0)
        {
            return _dbSet
                .AsNoTracking()
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                })
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }

        return _dbSet
            .AsNoTracking()
            .Where(c => c.FirstName.ToUpper().Contains(term)
                || c.LastName.ToUpper().Contains(term)
                || c.Phone.ToUpper().Contains(term))
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone,
            })
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToList();
    }

    public bool DeleteRangeById(List<int> rangeId)
    {
        var contacts = _dbSet
            .AsNoTracking()
            .Where(c => rangeId.Contains(c.Id))
            .ToList();

        _dbSet.RemoveRange(contacts);

        Save();

        return true;
    }

    public Contact? FindContactById(int id)
    {
        return _dbSet.FirstOrDefault(c => c.Id == id);
    }
}
