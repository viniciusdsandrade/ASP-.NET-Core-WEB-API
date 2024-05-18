namespace APICatalog.Models;

public record ProductDetailsDto(
    int ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    int Stock,
    DateTime CreatedDate,
    int CategoryId,
    string CategoryName
);