using PhoneBook.Model;

namespace PhoneBook.Contracts.Dto;

public class PhoneNumberDto
{
    public int Id { get; set; }

    public string? Phone { get; set; }

    public PhoneNumberType Type { get; set; }
}