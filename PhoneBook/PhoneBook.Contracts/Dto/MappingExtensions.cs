using PhoneBook.Model;

namespace PhoneBook.Contracts.Dto;

public static class MappingExtensions
{
    public static Contact ToModel(this ContactDto contactDto)
    {
        return new Contact
        {
            Id = contactDto.Id,
            FirstName = contactDto.FirstName,
            LastName = contactDto.LastName,
            Phone = contactDto.Phone
        };
    }

    public static ContactDto ToDto(this Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Phone = contact.Phone
        };
    }
}