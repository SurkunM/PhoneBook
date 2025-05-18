namespace PhoneBook.Contracts.IUnitOfWork;

public interface IUnitOfWorkTransaction
{
    void BeginTransaction();

    void RollbackTransaction();
}
