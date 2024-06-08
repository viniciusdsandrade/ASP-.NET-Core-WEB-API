using System.Linq.Expressions;
using APICatalog.Context;
using APICatalog.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Repositories.Generic;

public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
{
    private readonly AppDbContext _context;
    private DbSet<T> DbSet => _context.Set<T>();

    public RepositoryAsync(AppDbContext context) => _context = context;

    public async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<IQueryable<T>> GetAllQueryable() => await Task.Run(() => DbSet.AsQueryable());

    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            throw new EntityNotFoundException($"Entidade do tipo {typeof(T).Name} com ID {id} não encontrada.");
        return entity;
    }

    public async Task<T> CreateAsync(T entity)
    {
        DbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> DeleteByIdAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null)
            throw new EntityNotFoundException($"Entidade do tipo {typeof(T).Name} com ID {id} não encontrada.");
        DbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await DbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id); // Adaptar para a chave primária da entidade

    public async Task<int> CountAsync() => await DbSet.CountAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await DbSet.Where(predicate).ToListAsync();

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        await DbSet.FirstOrDefaultAsync(predicate);
}