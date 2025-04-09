namespace PhoneBook.Contracts.Repositories;

public interface IRepository
{
}


public interface IRepository<T> : IRepository
{
    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);

    void Save();
}