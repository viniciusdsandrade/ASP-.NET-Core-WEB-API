using APICatalog.Context;
using APICatalog.Models;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Generic;

namespace APICatalog.Repositories.UnitOfWork.Async;

public class UnitOfWorkAsync : IUnitOfWorkAsync
{
    private ICategoryRepositoryAsync _categoryRepositoryAsync;
    private IProductRepositoryAsync _productRepositoryAsync;
    private IRepositoryAsync<Category> _categoryRepositoryAsyncGeneric;
    private IRepositoryAsync<Product> _productRepositoryAsyncGeneric;
    private readonly AppDbContext _context;

    public UnitOfWorkAsync(AppDbContext context) => _context = context;

    public IProductRepositoryAsync ProductRepositoryAsync =>
        _productRepositoryAsync ??= new ProductRepositoryAsync(_context);

    public ICategoryRepositoryAsync CategoryRepositoryAsync =>
        _categoryRepositoryAsync ??= new CategoryRepositoryAsync(_context);

    public IRepositoryAsync<Product> ProductRepositoryAsyncGeneric =>
        _productRepositoryAsyncGeneric ??= new RepositoryAsync<Product>(_context);

    public IRepositoryAsync<Category> CategoryRepositoryAsyncGeneric =>
        _categoryRepositoryAsyncGeneric ??= new RepositoryAsync<Category>(_context);

    public Task CommitAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}