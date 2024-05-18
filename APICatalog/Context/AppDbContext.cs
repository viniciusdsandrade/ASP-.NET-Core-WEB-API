using APICatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("db_catalog_api");
    }

    public DbSet<Category> Category { get; set; }
    public DbSet<Product> Products { get; set; }
}