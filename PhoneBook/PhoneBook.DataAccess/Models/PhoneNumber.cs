namespace PhoneBook.DataAccess.Models;

public class PhoneNumber
{
    public int Id { get; set; }

    public string Phone { get; set; }

    public PhoneNumberType Type { get; set; }

    public int ContactId { get; set; }

    public virtual Contact Contact { get; set; }
}