namespace APICatalog.Models;

public record CategoryDetailsDto(
    int CategoryId,
    string Name,
    string ImageUrl,
    List<string> ProductNames
);