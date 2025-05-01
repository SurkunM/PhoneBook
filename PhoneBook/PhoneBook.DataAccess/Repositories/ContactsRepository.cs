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

    private int _pageNumber;

    private int _pageSize;

    private bool _isDescending;

    private string _orderByPropertyName = "";

    public ContactsRepository(PhoneBookDbContext dbContext, ILogger<ContactsRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    private static Expression<Func<ContactDto, object>> GetPropertyExpression(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(ContactDto), "c");

        var property = typeof(ContactDto).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
          ?? throw new ArgumentException($"Недопустимое имя свойства: {propertyName}");

        var access = Expression.MakeMemberAccess(parameter, property);

        return Expression.Lambda<Func<ContactDto, object>>(Expression.Convert(access, typeof(object)), parameter);
    }

    private Expression<Func<ContactDto, object>> CreateSortExpression(string propertyName)
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

    public async Task<PhoneBookPage> GetContactsAsync(string term)
    {
        var queryableSbSet = _dbSet.AsNoTracking();

        if (!string.IsNullOrEmpty(term))
        {
            term = term.Trim().ToUpper();
            queryableSbSet = queryableSbSet.Where(c => c.FirstName.ToUpper().Contains(term) || c.LastName.ToUpper().Contains(term) || c.Phone.ToUpper().Contains(term));
        }

        var queryableContactsDto = queryableSbSet
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Phone = c.Phone
            });

        var orderByExpression = CreateSortExpression(_orderByPropertyName);

        queryableContactsDto = _isDescending ? queryableContactsDto.OrderByDescending(orderByExpression) : queryableContactsDto.OrderBy(orderByExpression);

        var totalCount = await _dbSet.CountAsync();

        var contactsDtoSorted = await queryableContactsDto
            .Skip((_pageNumber - 1) * _pageSize)
            .Take(_pageSize)
            .ToListAsync();

        var page = new PhoneBookPage
        {
            ContactsDto = contactsDtoSorted,
            TotalCount = totalCount
        };

        SetRepositoryDefaultState();

        return page;
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

    public void SetSortingParameters(string orderBy, bool isDescending)
    {
        _orderByPropertyName = orderBy;
        _isDescending = isDescending;
    }

    public void SetPagingParameters(int pageNumber, int pageSize)
    {
        _pageNumber = pageNumber;
        _pageSize = pageSize;
    }

    public void SetRepositoryDefaultState()
    {
        _pageNumber = 1;
        _pageSize = 10;

        _isDescending = false;
        _orderByPropertyName = "FirstName";
    }
}
