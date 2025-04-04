using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess.Models;

namespace PhoneBook.DataAccess;

public class PhoneBookDbContext : DbContext
{
    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

    public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contact>(b =>
        {
            b.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            b.Property(c => c.LastName).IsRequired().HasMaxLength(50);
            b.Property(c => c.MiddleName).HasMaxLength(50);
        });

        modelBuilder.Entity<PhoneNumber>(b =>
        {
            b.Property(p => p.Phone).IsRequired().HasMaxLength(20);

            b.HasOne(p => p.Contact)
                .WithMany(c => c.PhoneNumbers)
                .HasForeignKey(p => p.ContactId);
        });
    }
}