namespace PhoneBook.Contracts.IUnitOfWork;

public interface IUnitOfWork : IDisposable
{
    Task SaveAsync();

    T GetRepository<T>() where T : class;
}
