using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Contracts.Dto;

public class ContactDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле фамилия не заполнено")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Поле имя не заполнено")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Поле телефон не заполнено")]
    [RegularExpression(@"^\+?\d+$", ErrorMessage = "Телефон должен содержать только цифры")]
    public required string Phone {  get; set; } 
}
