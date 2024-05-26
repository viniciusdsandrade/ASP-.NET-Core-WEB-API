using APICatalog.Models;

namespace APICatalog.Repositories.Async;

public interface ICategoryRepositoryAsync
{
    Task<Category> CreateAsync(Category category);
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize);
    Task<Category> UpdateAsync(Category category);
    Task<Category?> DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}