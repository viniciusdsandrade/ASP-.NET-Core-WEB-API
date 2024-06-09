using APICatalog.Context;
using APICatalog.Models;

namespace APICatalog.Repositories.Sync;

public class ProductRepositorySync : IProductRepositorySync
{
    private readonly AppDbContext _context;

    public ProductRepositorySync(AppDbContext context) => _context = context;

    public IQueryable<Product> GetAll()
    {
        return _context.Products;
    }

    public Product GetById(int id)
    {
        var produto = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (produto == null) throw new Exception("Product not found");
        return produto;
    }

    public Product Create(Product product)
    {
        if (product == null) throw new Exception("Product is null");

        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public bool Update(Product product)
    {
        if (product == null) throw new InvalidOperationException("Product is null");

        if (_context.Products.Any(p => p.ProductId == product.ProductId))
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    public bool Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

        if (product is null) return false;

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }
}