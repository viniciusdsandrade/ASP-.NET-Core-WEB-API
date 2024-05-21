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

    public ProductsController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<ProductDetailsDto>> PostProduct(ProductDto? productDto)
    {
        if (productDto is null) return BadRequest("Product data is null.");

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

    [HttpPut("{id:int}")]
    public ActionResult<ProductDetailsDto> PutProduct(int id, Product? product)
    {
        if (product is null) return BadRequest("Product data is null.");
        if (id != product.ProductId) return BadRequest("Product ID mismatch.");

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();

        // Carrega a categoria relacionada
        _context.Entry(product).Reference(p => p.Category).Load();

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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDetailsDto>> DeleteProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound("Product not found.");

        // Carrega a categoria relacionada
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

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

        return Ok(productDetailsDto);
    }

    [HttpGet("details/{id:int}", Name = "GetProductDetails")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category) // Carrega a categoria relacionada
            .AsNoTracking()
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

        return Ok(productDetailsDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProducts(int page = 1, int pageSize = 10)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        if (products.Count == 0) return NotFound("No products found.");

        var productDetailsDtos = products.Select(product => new ProductDetailsDto(
            product.ProductId,
            product.Name!,
            product.Description!,
            product.Price,
            product.ImageUrl!,
            product.Stock,
            product.CreatedDate,
            product.CategoryId,
            product.Category?.Name!
        ));

        return Ok(productDetailsDtos); // Corrigido: encapsulando em Ok()
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound("Product not found.");
        return product;
    }
}