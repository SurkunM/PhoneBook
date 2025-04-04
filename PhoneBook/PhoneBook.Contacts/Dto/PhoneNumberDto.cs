using PhoneBook.DataAccess.Models;

namespace PhoneBook.Contacts.Dto;

public class PhoneNumberDto
{
    public int Id { get; set; }

    public string? Phone { get; set; }

    public PhoneNumberType Type { get; set; }
}