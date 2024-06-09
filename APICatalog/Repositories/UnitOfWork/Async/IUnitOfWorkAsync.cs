using APICatalog.Models;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Generic;

namespace APICatalog.Repositories.UnitOfWork.Async;

public interface IUnitOfWorkAsync
{
    IProductRepositoryAsync ProductRepositoryAsync { get; }
    ICategoryRepositoryAsync CategoryRepositoryAsync { get; }
    IRepositoryAsync<Product> ProductRepositoryAsyncGeneric { get; }
    IRepositoryAsync<Category> CategoryRepositoryAsyncGeneric { get; }

    Task CommitAsync();
    void Dispose();
}