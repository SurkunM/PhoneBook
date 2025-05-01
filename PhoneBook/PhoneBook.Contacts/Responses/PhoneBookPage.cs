using PhoneBook.Contracts.Dto;

namespace PhoneBook.Contracts.Responses;

public class PhoneBookPage
{
    public List<ContactDto> ContactsDto { get; set; } = new List<ContactDto>();

    public int TotalCount { get; set; }
}
