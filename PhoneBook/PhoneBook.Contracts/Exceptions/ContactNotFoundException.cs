namespace PhoneBook.Contracts.Exceptions;

public class ContactNotFoundException : Exception
{
    public ContactNotFoundException(string message) : base(message) { }

    public ContactNotFoundException(string message, Exception exception) : base(message, exception) { }
}
