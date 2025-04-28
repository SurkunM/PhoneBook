using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    public bool IsDescending { get ; set ; }

    public string OrderBy { get; set; } = default!;

    public ContactsRepository(PhoneBookDbContext dbContext) : base(dbContext) { }

    public async Task<List<ContactDto>> GetContactsAsync(string term)
    {
        term = term.Trim();

        if (string.IsNullOrEmpty(term))
        {
            return await _dbSet
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
                .ToListAsync();
        }

        term = term.ToUpper();

        return await _dbSet
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
            .ToListAsync();
    }

    public async Task<bool> DeleteRangeByIdAsync(List<int> rangeId)
    {
        var contacts = await _dbSet
            .AsNoTracking()
            .Where(c => rangeId.Contains(c.Id))
            .ToListAsync();

        _dbSet.RemoveRange(contacts);

        await SaveAsync();

        return true;
    }

    public async Task<Contact?> FindContactByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> CheckIsPhoneExistAsync(ContactDto contactDto)
    {
        return await _dbSet.AnyAsync(c => c.Id != contactDto.Id && c.Phone == contactDto.Phone);
    }
}
