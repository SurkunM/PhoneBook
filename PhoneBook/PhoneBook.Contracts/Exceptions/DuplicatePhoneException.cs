namespace PhoneBook.Contracts.Exceptions;

public class DuplicatePhoneException : Exception
{
    public DuplicatePhoneException(string message) : base(message) { }

    public DuplicatePhoneException(string message, Exception exception) : base(message, exception) { }
}
