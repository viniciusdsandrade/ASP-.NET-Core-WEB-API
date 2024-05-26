using APICatalog.Context;
using APICatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Repositories.Async;

public class CategoryRepositoryAsync : ICategoryRepositoryAsync
{
    private readonly AppDbContext _context;

    public CategoryRepositoryAsync(AppDbContext context) => _context = context;

    public async Task<Category> CreateAsync(Category category)
    {
        await _context.Category.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Category
            .Include(c => c.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Category
            .Include(c => c.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> DeleteAsync(int id)
    {
        var category = await _context.Category.FindAsync(id);

        if (category == null) return category;

        _context.Category.Remove(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}