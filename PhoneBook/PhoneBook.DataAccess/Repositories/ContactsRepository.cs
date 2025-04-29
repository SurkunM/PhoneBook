using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    public bool IsDescending { get; set; }

    public string OrderByProperty { get; set; } = default!;

    public ContactsRepository(PhoneBookDbContext dbContext) : base(dbContext) { }

    private static Expression<Func<ContactDto, object>> CreateSortExpression(string propertyName)
    {
        var propertyParameter = Expression.Parameter(typeof(ContactDto), "c");

        var property = typeof(ContactDto)
            .GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
            ?? throw new ArgumentException($"Передано не допустимое значение: {propertyName}");

        var propertyAccess = Expression.MakeMemberAccess(propertyParameter, property);

        return Expression.Lambda<Func<ContactDto, object>>(Expression.Convert(propertyAccess, typeof(object)), propertyParameter);
    }

    public async Task<List<ContactDto>> GetContactsAsync(string term)
    {
        var queryableSbSet = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(term))
        {
            term = term.Trim().ToUpper();
            queryableSbSet.Where(c => c.FirstName.ToUpper().Contains(term) || c.LastName.ToUpper().Contains(term) || c.Phone.ToUpper().Contains(term));
        }

        var queryableDto = queryableSbSet
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone
                });

        var orderByExpression = CreateSortExpression(OrderByProperty);

        queryableDto = IsDescending ? queryableDto.OrderByDescending(orderByExpression) : queryableDto.OrderBy(orderByExpression);

        return await queryableDto.ToListAsync();
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
