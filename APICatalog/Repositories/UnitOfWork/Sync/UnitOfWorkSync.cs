using APICatalog.Context;
using APICatalog.Models;
using APICatalog.Repositories.Generic;
using APICatalog.Repositories.Sync;

namespace APICatalog.Repositories.UnitOfWork.Sync;

public class UnitOfWorkSync : IUnitOfWorkSync
{
    private ICategoryRepositorySync _categoryRepositorySync;
    private IProductRepositorySync _productRepositorySync;
    private IRepositorySync<Product> _productRepositoryGeneric;
    private IRepositorySync<Category> _categoryRepositoryGeneric;
    private readonly AppDbContext _context;

    public UnitOfWorkSync(AppDbContext context) => _context = context;

    public ICategoryRepositorySync CategoryRepositorySync =>
        _categoryRepositorySync ??= new CategoryRepositorySync(_context);

    public IProductRepositorySync ProductRepositorySync =>
        _productRepositorySync ??= new ProductRepositorySync(_context);

    public IRepositorySync<Product> ProductRepositoryGeneric =>
        _productRepositoryGeneric ??= new RepositorySync<Product>(_context);

    public IRepositorySync<Category> CategoryRepositoryGeneric =>
        _categoryRepositoryGeneric ??= new RepositorySync<Category>(_context);

    public void Commit() => _context.SaveChanges();

    public void Dispose() => _context.Dispose();
}