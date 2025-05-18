using Microsoft.EntityFrameworkCore;
using PhoneBook.Model;

namespace PhoneBook.DataAccess;

public class PhoneBookDbContext : DbContext
{
    public virtual DbSet<Contact> Contacts { get; set; }

    public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(b =>
        {
            b.Property(c => c.FirstName).HasMaxLength(50);
            b.Property(c => c.LastName).HasMaxLength(50);            
            b.Property(c => c.Phone).HasMaxLength(50);
        });
    }
}