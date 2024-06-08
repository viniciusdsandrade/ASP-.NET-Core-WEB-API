namespace APICatalog.Models.Dtos;

public record ProductDto(
    string? Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    int Stock,
    int CategoryId
);