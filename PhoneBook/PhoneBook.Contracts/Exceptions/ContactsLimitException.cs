namespace PhoneBook.Contracts.Exceptions;

public class ContactsLimitException : Exception
{
    public ContactsLimitException(string message) : base(message) { }

    public ContactsLimitException(string message, Exception exception) : base(message, exception) { }
}
