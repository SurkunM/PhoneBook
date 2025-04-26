using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.DataAccess.Repositories.BaseAbstractions;

public class BaseEfRepository<T> : IRepository<T> where T : class
{
    protected PhoneBookDbContext _dbContext;

    protected DbSet<T> _dbSet;

    public BaseEfRepository(PhoneBookDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);

        await SaveAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);

        await SaveAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;

        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
