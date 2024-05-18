using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalog.Models;

public class Product
{
    public Product()
    {
        CreatedDate = DateTime.Now;
    }

    [Key] public int ProductId { get; init; }

    [Required] [StringLength(80)] public string? Name { get; init; }
    [Required] [StringLength(300)] public string? Description { get; init; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; init; }

    [Required] [StringLength(300)] public string? ImageUrl { get; init; }
    public int Stock { get; init; }

    public DateTime CreatedDate { get; init; }

    // Incluimos uma propriedade 'CategoryId' que mapeia para a chave estrangeira no banco de dados e uma propriedade de navegação 'Category' para indicar que um Produto está relacionado com UMA Categoria
    public int CategoryId { get; init; }
    public Category? Category { get; init; }
}