using APICatalog.Models;

namespace APICatalog.Repositories.Sync;

public interface IProductRepositorySync
{
    IQueryable<Product> GetAll();
    Product GetById(int id);
    Product Create(Product product);
    bool Update(Product product);
    bool Delete(int id);
}