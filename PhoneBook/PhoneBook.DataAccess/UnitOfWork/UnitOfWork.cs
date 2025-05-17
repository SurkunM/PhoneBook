using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _db;

    private readonly IServiceProvider _serviceProvider;

    private bool _disposed;

    public UnitOfWork(PhoneBookDbContext db, IServiceProvider serviceProvider)
    {
        _db = db;
        _serviceProvider = serviceProvider;
    }

    public T GetRepository<T>() where T : class
    {
        ThrowExceptionIfDisposed();

        return _serviceProvider.GetRequiredService<T>();
    }

    public Task SaveAsync()
    {
        ThrowExceptionIfDisposed();

        return _db.SaveChangesAsync();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _db.Dispose();
        _disposed = true;
    }

    private void ThrowExceptionIfDisposed()
    {
        if (!_disposed)
        {
            return;
        }

        throw new ObjectDisposedException(null);
    }
}
