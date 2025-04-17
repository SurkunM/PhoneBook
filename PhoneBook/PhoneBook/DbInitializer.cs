using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess;
using PhoneBook.Model;

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

        if (_dbContext.Contacts.Any())
        {
            return;
        }

        _dbContext.Contacts.Add(new Contact
        {
            FirstName = "Иван",
            LastName = "Иванов",
            Phone = "9139990000"
        });

        _dbContext.Contacts.Add(new Contact
        {
            FirstName = "Сергей",
            LastName = "Сергеев",
            Phone = "9139991111"
        });

        _dbContext.SaveChanges();
    }
}