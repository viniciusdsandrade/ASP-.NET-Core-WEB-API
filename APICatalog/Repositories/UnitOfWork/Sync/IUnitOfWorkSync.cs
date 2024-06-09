using APICatalog.Models;
using APICatalog.Repositories.Generic;
using APICatalog.Repositories.Sync;

namespace APICatalog.Repositories.UnitOfWork.Sync;

public interface IUnitOfWorkSync
{
    ICategoryRepositorySync CategoryRepositorySync { get; }
    IProductRepositorySync ProductRepositorySync { get; }
    IRepositorySync<Category> CategoryRepositoryGeneric { get; }
    IRepositorySync<Product> ProductRepositoryGeneric { get; }
    void Commit();
    void Dispose();
}