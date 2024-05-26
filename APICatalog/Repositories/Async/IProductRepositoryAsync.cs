using APICatalog.Models;

namespace APICatalog.Repositories.Async;

public interface IProductRepositoryAsync
{
    Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize);
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(int id, Product product);
    Task<Product?> DeleteProductAsync(int id);
}