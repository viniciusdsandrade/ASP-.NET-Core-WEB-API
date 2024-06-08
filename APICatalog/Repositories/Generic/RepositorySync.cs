using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using APICatalog.Context;
using APICatalog.Exceptions;

namespace APICatalog.Repositories.Generic;

public class RepositorySync<T> : IRepositorySync<T> where T : class
{
    private readonly AppDbContext _context;
    private DbSet<T> DbSet => _context.Set<T>();

    public RepositorySync(AppDbContext context) => _context = context;

    public IEnumerable<T?> GetAll() => DbSet.ToList();

    public IQueryable<T?> GetAllQueryable() => DbSet;

    public T? GetById(int id) => DbSet.Find(id);

    public T Create(T? entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        DbSet.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public T Update(T? entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        DbSet.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public T DeleteById(int id)
    {
        var entity = GetById(id);
        if (entity == null)
            throw new EntityNotFoundException($"Entidade do tipo {typeof(T).Name} com ID {id} não encontrada.");
        DbSet.Remove(entity);
        _context.SaveChanges();
        return entity;
    }

    public bool Exists(int id)
    {
        // Check if any entity with the given ID exists (assuming your entities have an "Id" property)
        return DbSet.Any(e => EF.Property<int>(e, "Id") == id);
    }

    public int Count() => DbSet.Count();

    public IEnumerable<T?> Find(Expression<Func<T?, bool>> predicate) => DbSet.Where(predicate).ToList();

    public T? FirstOrDefault(Expression<Func<T?, bool>> predicate) => DbSet.FirstOrDefault(predicate);
}