using APICatalog.Context;
using APICatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context) => _context = context;

    // POST api/v1/products
    [HttpPost]
    public async Task<ActionResult<ProductDetailsDto>> CreateProduct(ProductDto? productDto)
    {
        if (productDto == null) return BadRequest("Product data is null.");

        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Stock = productDto.Stock,
            CategoryId = productDto.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Usamos o construtor do DTO para criar o objeto
        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.ProductId },
            new ProductDetailsDto(product, includeCategoryName: true)
        );
    }

    // PUT api/v1/products/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDetailsDto>> UpdateProduct(int id, ProductDto? productDto)
    {
        if (productDto == null) return BadRequest("Product data is null.");
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null) return NotFound("Product not found.");

        // Atualiza as propriedades do produto existente
        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.ImageUrl = productDto.ImageUrl;
        existingProduct.Stock = productDto.Stock;
        existingProduct.CategoryId = productDto.CategoryId;

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = existingProduct.ProductId },
            new ProductDetailsDto(existingProduct, includeCategoryName: true)
        );
    }

    // DELETE api/v1/products/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDetailsDto>> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new ProductDetailsDto(product, includeCategoryName: true));
    }

    // GET api/v1/products/{id}
    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null) return NotFound("Product not found.");

        return Ok(new ProductDetailsDto(product, includeCategoryName: true));
    }

    // GET api/v1/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page <= 0) return BadRequest("Page must be greater than zero.");
        if (pageSize is <= 0 or > 100) return BadRequest("Page size must be between 1 and 100.");

        var products = await _context.Products
            .Include(p => p.Category)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        if (products.Count == 0) return NotFound("No products found for the specified page.");

        // Cria uma lista de DTOs usando o construtor
        var productDetailsDtos = products.Select(p => new ProductDetailsDto(p, includeCategoryName: true));
        return Ok(productDetailsDtos);
    }

    // GET api/v1/products/name/{name}
    [HttpGet("name/{name}")]
    public async Task<ActionResult<IEnumerable<ProductDetailsDto>>> GetProductsByName(string name)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name!.Contains(name))
            .AsNoTracking()
            .ToListAsync();

        if (products.Count == 0) return NotFound("No products found with the specified name.");

        // Cria uma lista de DTOs usando o construtor
        var productDetailsDtos = products.Select(p => new ProductDetailsDto(p, includeCategoryName: true));
        return Ok(productDetailsDtos);
    }
}