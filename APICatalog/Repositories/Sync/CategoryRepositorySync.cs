using APICatalog.Context;
using APICatalog.Models;
using APICatalog.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Repositories.Sync;

public class CategoryRepositorySync : RepositorySync<Category>, ICategoryRepositorySync
{
    private readonly AppDbContext _context;

    public CategoryRepositorySync(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAll(int page, int pageSize)
    {
        return _context.Category
            .Include(c => c.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToList();
    }

    public new Category GetById(int id)
    {
        return _context.Category
                   .Include(c => c.Products)
                   .AsNoTracking()
                   .FirstOrDefault(c => c.CategoryId == id)
               ?? throw new Exception($"Categoria não encontrada com ID: {id}");
    }

    public new Category Create(Category category)
    {
        _context.Category.Add(category);
        _context.SaveChanges();
        return category;
    }

    public new Category Update(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();
        return category;
    }

    public void Delete(int id)
    {
        var category = _context.Category.Find(id);

        // Se a categoria não for encontrada, lanço uma exceção
        if (category is null) throw new Exception($"Categoria não encontrada com o ID: {id}");

        // Preciso verificar se esta categorias prestes a ser excluída não possui produtos associados
        if (category.Products.Count > 0)
            throw new Exception("Não é possível excluir uma categoria com produtos associados.");

        _context.Category.Remove(category);
        _context.SaveChanges();
    }
}