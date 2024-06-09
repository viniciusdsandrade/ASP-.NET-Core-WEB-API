using APICatalog.Models;
using APICatalog.Models.Dtos;
using APICatalog.Repositories.Async;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Async;

[ApiController]
[Route("api/v1/async/[controller]")]
public class ProductsControllerAsync(
    IProductRepositoryAsync repository,
    ILogger<ProductsControllerAsync> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        logger.LogInformation("Obtendo produtos. Página: {page}, Tamanho da página: {pageSize}", page, pageSize);

        if (page <= 0) return BadRequest("Page must be greater than zero.");
        if (pageSize is <= 0 or > 100) return BadRequest("Page size must be between 1 and 100.");

        var products = await repository.GetProductsAsync(page, pageSize);

        var productsList = products.ToList();

        if (productsList.Count == 0) return NotFound("No products found for the specified page.");

        var productDetailsDtos = productsList.Select(p => new ProductDetailsDto(p, includeCategoryName: true));
        return Ok(productDetailsDtos);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProductsByName(string name)
    {
        logger.LogInformation("Obtendo produtos por nome: {name}", name);

        var products = await repository.GetProductsByNameAsync(name);

        var productsList = products.ToList();

        if (productsList.Count == 0) return NotFound("No products found with the specified name.");

        var productDetailsDtos = productsList.Select(p => new ProductDetailsDto(p, includeCategoryName: true));
        return Ok(productDetailsDtos);
    }

    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductById(int id)
    {
        logger.LogInformation("Obtendo produto por ID: {id}", id);

        var product = await repository.GetProductByIdAsync(id);

        if (product == null)
        {
            logger.LogWarning("Produto com ID {id} não encontrado.", id);
            return NotFound("Product not found.");
        }

        return Ok(new ProductDetailsDto(product, includeCategoryName: true));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDetailsDto>> CreateProduct(ProductDto? productDto)
    {
        logger.LogInformation("Criando novo produto.");

        if (productDto == null)
        {
            logger.LogWarning("Requisição para criar produto recebida com dados inválidos.");
            return BadRequest("Product data is null.");
        }

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Stock = productDto.Stock,
            CategoryId = productDto.CategoryId
        };

        var createdProduct = await repository.CreateProductAsync(product);
        var productDetailsDto = new ProductDetailsDto(createdProduct, includeCategoryName: true);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = createdProduct.ProductId },
            productDetailsDto
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDetailsDto>> UpdateProduct(int id, ProductDto? productDto)
    {
        logger.LogInformation("Atualizando produto com ID {id}.", id);

        if (productDto == null)
        {
            logger.LogWarning("Requisição para atualizar produto com ID {id} recebida com dados inválidos.", id);
            return BadRequest("Product data is null.");
        }

        var product = new Product
        {
            ProductId = id,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Stock = productDto.Stock,
            CategoryId = productDto.CategoryId
        };

        var updatedProduct = await repository.UpdateProductAsync(id, product);

        if (updatedProduct == null)
        {
            logger.LogWarning("Produto com ID {id} não encontrado para atualização.", id);
            return NotFound("Product not found.");
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = updatedProduct.ProductId },
            new ProductDetailsDto(updatedProduct, includeCategoryName: true)
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDetailsDto>> DeleteProduct(int id)
    {
        logger.LogInformation("Excluindo produto com ID {id}.", id);

        var deletedProduct = await repository.DeleteProductAsync(id);

        if (deletedProduct == null)
        {
            logger.LogWarning("Produto com ID {id} não encontrado para exclusão.", id);
            return NotFound("Product not found.");
        }

        return Ok(new ProductDetailsDto(deletedProduct, includeCategoryName: true));
    }
}