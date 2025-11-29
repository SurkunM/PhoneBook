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

    private const int MAX_CONTACTS_LIMMIT = 1000;

    public ContactsRepository(PhoneBookDbContext dbContext, ILogger<ContactsRepository> logger) : base(dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PhoneBookPage> GetContactsAsync(GetContactsQueryParameters queryParameters)
    {
        await using var transaction = await DbContext.Database.BeginTransactionAsync();

        try
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
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone
                })
                .ToListAsync();

            var totalCount = contactsDtoSorted.Count;

            if (string.IsNullOrEmpty(queryParameters.Term))
            {
                totalCount = await DbSet.CountAsync();
            }

            await transaction.CommitAsync();

            return new PhoneBookPage
            {
                Contacts = contactsDtoSorted,
                TotalCount = totalCount
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            throw;
        }
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

    public async Task DeleteRangeByIdAsync(List<int> idsRange)
    {
        var contacts = await DbSet
            .AsNoTracking()
            .Where(c => idsRange.Contains(c.Id))
            .ToListAsync();

        DbSet.RemoveRange(contacts);
    }

    public Task<Contact?> FindContactByIdAsync(int id)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<bool> IsPhoneExistAsync(ContactDto contactDto)
    {
        return DbSet.AnyAsync(c => c.Id != contactDto.Id && c.Phone == contactDto.Phone);
    }

    public async Task<bool> CheckContactLimitAsync()
    {
        var contactsCount = await DbSet.CountAsync();

        return contactsCount < MAX_CONTACTS_LIMMIT;
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
}
