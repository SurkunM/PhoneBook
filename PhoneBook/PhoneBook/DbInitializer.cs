using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess;
using PhoneBook.DataAccess.Models;

namespace PhoneBook;

public class DbInitializer
{
    private readonly PhoneBookDbContext _dbContext;

    public DbInitializer(PhoneBookDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public void Initialize()
    {
        _dbContext.Database.Migrate();

        if (!_dbContext.Contacts.Any())
        {
            _dbContext.Add(new Contact
            {
                FirstName = "Иван",
                LastName = "Иванов",
                MiddleName = "Иванович",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Phone = "9139990000",
                        Type = PhoneNumberType.Mobile
                    }
                }
            });

            _dbContext.SaveChanges();
        }
    }
}