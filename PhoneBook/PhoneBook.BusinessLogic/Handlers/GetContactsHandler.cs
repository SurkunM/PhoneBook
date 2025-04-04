using Microsoft.EntityFrameworkCore;
using PhoneBook.Contacts.Dto;
using PhoneBook.DataAccess;

namespace PhoneBook.BusinessLogic.Handlers;

public class GetContactsHandler
{
    private readonly PhoneBookDbContext _dbContext;

    public GetContactsHandler(PhoneBookDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public List<ContactDto> Handle()
    {
        return _dbContext.Contacts
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
