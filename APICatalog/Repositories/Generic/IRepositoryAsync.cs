using System.Linq.Expressions;

namespace APICatalog.Repositories.Generic;

public interface IRepositoryAsync<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IQueryable<T>> GetAllQueryable();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T?> DeleteByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
}