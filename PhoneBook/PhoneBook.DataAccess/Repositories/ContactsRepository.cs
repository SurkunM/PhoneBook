using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.Contracts.Responses;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    private readonly ILogger<ContactsRepository> _logger;

    public ContactsRepository(PhoneBookDbContext dbContext, ILogger<ContactsRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    private static Expression<Func<Contact, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(Contact), "c");

        var property = typeof(Contact).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
          ?? throw new ArgumentException($"Недопустимое имя свойства: {propertyName}");

        var access = Expression.MakeMemberAccess(parameter, property);

        return Expression.Lambda<Func<Contact, object>>(Expression.Convert(access, typeof(object)), parameter);
    }

    private Expression<Func<Contact, object>> CreateSortExpression(string propertyName)
    {
        try
        {
            return GetPropertyExpression(propertyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Не удалось сформировать выражение для параметра сортировки. Использовано поле 'LastName' по умолчанию");

            return GetPropertyExpression("LastName");
        }
    }

    public async Task<PhoneBookPage> GetContactsAsync(GetContactsQueryParameters queryParameters)
    {
        var querySbSet = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim().ToUpper();
            querySbSet = querySbSet.Where(c => c.FirstName.ToUpper().Contains(queryParameters.Term)
                || c.LastName.ToUpper().Contains(queryParameters.Term)
                || c.Phone.ToUpper().Contains(queryParameters.Term));
        }

        var orderByExpression = CreateSortExpression(queryParameters.SortBy);

        var orderedQuery = queryParameters.IsDescending
            ? querySbSet.OrderByDescending(orderByExpression)
            : querySbSet.OrderBy(orderByExpression);

        var contactsDtoSorted = await orderedQuery
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .Select((c) => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone
            })
            .ToListAsync();

        for (int i = 0; i < contactsDtoSorted.Count; i++)
        {
            contactsDtoSorted[i].Index = (queryParameters.PageNumber - 1) * queryParameters.PageSize + i + 1; ;
        }

        var totalCount = await _dbSet.CountAsync();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            totalCount = contactsDtoSorted.Count;
        }

        return new PhoneBookPage
        {
            Contacts = contactsDtoSorted,
            TotalCount = totalCount
        };
    }

    public async Task<List<ContactDto>> GetContactsAsync()
    {
        return await _dbSet.AsNoTracking()
            .Select((c) => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone
            })
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
