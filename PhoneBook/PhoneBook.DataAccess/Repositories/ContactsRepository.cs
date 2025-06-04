using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.IRepositories;
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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        var querySbSet = DbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParameters.Term))
        {
            queryParameters.Term = queryParameters.Term.Trim();
            querySbSet = querySbSet.Where(c => c.FirstName.Contains(queryParameters.Term)
                || c.LastName.Contains(queryParameters.Term)
                || c.Phone.Contains(queryParameters.Term));
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

        var totalCount = DbSet.Count();

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

    public Task<List<ContactDto>> GetContactsAsync()
    {
        return DbSet.AsNoTracking()
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    public async Task DeleteRangeByIdAsync(List<int> rangeId)
    {
        var contacts = await DbSet
            .AsNoTracking()
            .Where(c => rangeId.Contains(c.Id))
            .ToListAsync();

        DbSet.RemoveRange(contacts);
    }

    public Task<Contact?> FindContactByIdAsync(int id)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id == id);
    }

    public bool IsPhoneExist(ContactDto contactDto)
    {
        return DbSet.Any(c => c.Id != contactDto.Id && c.Phone == contactDto.Phone);
    }
}
