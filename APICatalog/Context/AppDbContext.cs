using APICatalog.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace APICatalog.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("db_catalog_api");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                "Server=localhost;Database=db_catalog_api;Uid=root;Pwd=GhostSthong567890@",
                ServerVersion.AutoDetect("Server=localhost;Database=db_catalog_api;Uid=root;Pwd=GhostSthong567890@"),
                options => options.SchemaBehavior(MySqlSchemaBehavior.Ignore));
        }
    }

    public DbSet<Category> Category { get; init; }
    public DbSet<Product> Products { get; init; }
}