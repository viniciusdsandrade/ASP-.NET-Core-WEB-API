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

    public IEnumerable<T?> GetAll() => DbSet.AsNoTracking().ToList(); // AsNoTracking adicionado para otimização

    public IQueryable<T?> GetAllQueryable() => DbSet.AsNoTracking(); // AsNoTracking adicionado para otimização

    public T? GetById(int id) => DbSet.Find(id); // Mantido Find para chave primária

    public T Create(T? entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        DbSet.Add(entity);
        return entity; // Removido SaveChanges, pois será chamado no UnitOfWork
    }

    public T Update(T? entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        DbSet.Update(entity);
        return entity; // Removido SaveChanges, pois será chamado no UnitOfWork
    }

    public T DeleteById(int id)
    {
        var entity = GetById(id);
        if (entity == null)
            throw new EntityNotFoundException($"Entidade do tipo {typeof(T).Name} com ID {id} não encontrada.");
        DbSet.Remove(entity);
        return entity; // Removido SaveChanges, pois será chamado no UnitOfWork
    }

    public bool Exists(int id)
    {
        return DbSet.Any(e => EF.Property<int>(e, "Id") == id);
    }

    public int Count() => DbSet.Count();

    public IEnumerable<T?> Find(Expression<Func<T?, bool>> predicate) => DbSet.Where(predicate).ToList();

    public T? FirstOrDefault(Expression<Func<T?, bool>> predicate) => DbSet.FirstOrDefault(predicate);
}