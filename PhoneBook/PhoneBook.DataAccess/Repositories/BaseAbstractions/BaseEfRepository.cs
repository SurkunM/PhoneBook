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

    public void Create(T entity)
    {
        _dbSet.Add(entity);

        Save();
    }

    public void Delete(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);

        Save();
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;

        Save();
    }

    public void Save()
    {
        _dbContext.SaveChanges();
    }
}
