using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using APICatalog.Validations;

namespace APICatalog.Models;

public class Product
{
    public Product() => CreatedDate = DateTime.Now;

    [Key] public int ProductId { get; init; }

    [Required(ErrorMessage = "O nome é Obrigatório")]
    [StringLength(80, ErrorMessage = "O nome deve ter no máximo 80 caracteres")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "A descrição é Obrigatória")]
    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "O preço é Obrigatório")]
    [Column(TypeName = "decimal(10,2)")]
    [PriceValidation(ErrorMessage = "O preço não pode ser negativo.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "A URL da imagem é Obrigatória")]
    [StringLength(300, ErrorMessage = "A URL da imagem deve ter no máximo 300 caracteres")]
    public string? ImageUrl { get; set; }

    public int Stock { get; set; }

    public DateTime CreatedDate { get; init; }
    // public DateTime? UpdatedDate { get; init; }

    // Incluimos uma propriedade 'CategoryId' que mapeia para a chave estrangeira no banco de dados e
    // uma propriedade de navegação 'Category' para indicar que um Produto está relacionado com UMA Categoria
    public int CategoryId { get; set; }
    [JsonIgnore] public Category? Category { get; init; }
}