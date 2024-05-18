using APICatalog.Context;
using APICatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    /*
     Diferença entre CreatedAtRouteResult e CreatedAtAction:
    1 - CreatedAtRouteResult: usado quando você precisa referenciar uma rota pelo seu nome, definido pelo atributo Name no atributo de roteamento.
    2 - CreatedAtAction: usado quando você quer referenciar uma ação pelo seu nome do método e, opcionalmente, pelo nome do controlador.
     */
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDetailsDto>> PostProduct(ProductDto productDto)
    {
        if (productDto is null)
            return BadRequest("Product data is null.");

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Stock = productDto.Stock,
            CategoryId = productDto.CategoryId
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Carrega a categoria relacionada
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();

        var productDetailsDto = new ProductDetailsDto(
            product.ProductId,
            product.Name!,
            product.Description!,
            product.Price,
            product.ImageUrl!,
            product.Stock,
            product.CreatedDate,
            product.CategoryId,
            product.Category?.Name ?? string.Empty // Obtem o nome da categoria
        );

        return CreatedAtAction(nameof(GetProductDetails), new { id = product.ProductId }, productDetailsDto);
    }

    [HttpGet("details/{id:int}", Name = "GetProductDetails")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category) // Carrega a categoria relacionada
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return NotFound("Product not found.");

        var productDetailsDto = new ProductDetailsDto(
            product.ProductId,
            product.Name!,
            product.Description!,
            product.Price,
            product.ImageUrl!,
            product.Stock,
            product.CreatedDate,
            product.CategoryId,
            product.Category?.Name! // Inclui o nome da categoria
        );

        return productDetailsDto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int page = 1, int pageSize = 10)
    {
        var products = await _context.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (products.Count == 0) return NotFound("No products found.");

        return products;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
            return NotFound("Product not found.");

        return product;
    }
}