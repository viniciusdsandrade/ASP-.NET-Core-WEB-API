namespace APICatalog.Models;

public class CategoryDetailsDto
{
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public List<string?> ProductNames { get; set; }

    public CategoryDetailsDto(Category category)
    {
        CategoryId = category.CategoryId;
        Name = category.Name;
        ImageUrl = category.ImageUrl;
        ProductNames = category.Products?.Select(p => p.Name).ToList() ?? [];
    }
}