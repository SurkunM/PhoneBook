using PhoneBook.Contracts.Dto;

namespace PhoneBook.Contracts.Responses;

public class PhoneBookPage
{
    public List<ContactDto> Contacts { get; set; } = new List<ContactDto>();

    public int TotalCount { get; set; }
}
