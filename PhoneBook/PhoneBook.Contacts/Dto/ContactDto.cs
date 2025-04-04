namespace PhoneBook.Contacts.Dto;

public class ContactDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public List<PhoneNumberDto> PhoneNumbers { get; set; }
}
