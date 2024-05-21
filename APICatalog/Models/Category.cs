using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace APICatalog.Models;

public class Category
{
    public Category() => Products = new Collection<Product>();

    [Key] public int CategoryId { get; init; }

    [Required(ErrorMessage = "O nome é Obrigatório")]
    [StringLength(80, ErrorMessage = "O nome deve ter no máximo 80 caracteres")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "A descrição é Obrigatória")]
    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres")]
    public string? ImageUrl { get; set; }

    // Incluimos uma propriedade ede navegação onde definimos que uma Categori pode conter uma coleção de Produtos
    public ICollection<Product>? Products { get; init; }
}