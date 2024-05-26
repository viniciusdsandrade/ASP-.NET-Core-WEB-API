using APICatalog.Models;

namespace APICatalog.Repositories.Sync;

public interface ICategoryRepositorySync
{
    IEnumerable<Category> GetAll(int page, int pageSize);
    Category GetById(int id);
    Category Create(Category category);
    Category Update(Category category);
    Category Delete(int id);
}