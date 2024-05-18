using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace APICatalog.Models;

public class Category
{
    public Category()
    {
        Products = new Collection<Product>();
    }

    [Key] public int CategoryId { get; init; }
    [Required] [StringLength(80)] public string? Name { get; init; }
    [Required] [StringLength(300)] public string? ImageUrl { get; init; }

    // Incluimos uma propriedade ede navegação onde definimos que uma Categoria pode conter uma coleção de Produtos
    public ICollection<Product>? Products { get; init; }
}