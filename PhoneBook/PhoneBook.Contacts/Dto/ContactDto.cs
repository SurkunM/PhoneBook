namespace PhoneBook.Contracts.Dto;

public class ContactDto
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Phone {  get; set; } 
}
