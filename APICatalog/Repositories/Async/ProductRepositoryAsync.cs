using APICatalog.Context;
using APICatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Repositories.Async;

public class ProductRepositoryAsync : IProductRepositoryAsync
{
    private readonly AppDbContext _context;

    public ProductRepositoryAsync(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name!.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null) return null;

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.Stock = product.Stock;
        existingProduct.CategoryId = product.CategoryId;

        await _context.SaveChangesAsync();

        return existingProduct;
    }

    public async Task<Product?> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return null;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return product;
    }
}