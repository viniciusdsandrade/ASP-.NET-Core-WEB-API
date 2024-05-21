namespace APICatalog.Models;

public class ProductDetailsDto
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public ProductDetailsDto(Product product, bool includeCategoryName = false)
    {
        ProductId = product.ProductId;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        ImageUrl = product.ImageUrl;
        Stock = product.Stock;
        CreatedDate = product.CreatedDate;
        CategoryId = product.CategoryId;

        if (includeCategoryName && product.Category != null) CategoryName = product.Category.Name;
    }
}