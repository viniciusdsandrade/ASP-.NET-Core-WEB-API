using APICatalog.Context;
using APICatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<CategoryDetailsDto>> CreateCategory(CategoryDto? categoryDto)
    {
        if (categoryDto is null) return BadRequest("Category data is null.");

        var category = new Category
        {
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl
        };

        await _context.Category.AddAsync(category);
        await _context.SaveChangesAsync();

        // Cria o DTO usando o construtor
        var categoryDetailsDto = new CategoryDetailsDto(category);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, categoryDetailsDto);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<ActionResult<CategoryDetailsDto>> GetCategoryById(int id)
    {
        var category = await _context.Category
            .Include(c => c.Products)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category is null) return NotFound($"Category with id {id} not found.");

        // Cria o DTO usando o construtor
        var categoryDetailsDto = new CategoryDetailsDto(category);

        return Ok(categoryDetailsDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDetailsDto>>> GetCategories(int page = 1, int pageSize = 10)
    {
        var categories = await _context.Category
            .Include(c => c.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        if (categories.Count == 0) return NotFound("No categories found.");

        // Cria uma lista de DTOs usando o construtor
        var categoriesDetailsDto = categories.Select(c => new CategoryDetailsDto(c));

        return Ok(categoriesDetailsDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> UpdateCategory(int id, CategoryDto? categoryDto)
    {
        if (categoryDto is null) return BadRequest("Category data is null.");
        var existingCategory = await _context.Category.FindAsync(id);
        if (existingCategory is null) return NotFound($"Category with id {id} not found.");

        existingCategory.Name = categoryDto.Name;
        existingCategory.ImageUrl = categoryDto.ImageUrl;

        await _context.SaveChangesAsync();

        // Recarrega os produtos relacionados após a atualização
        await _context.Entry(existingCategory)
            .Collection(c => c.Products!)
            .Query()
            .AsNoTracking()
            .LoadAsync();

        // Cria o DTO usando o construtor
        var categoryDetailsDto = new CategoryDetailsDto(existingCategory);

        return CreatedAtAction(nameof(GetCategoryById), new { id = existingCategory.CategoryId }, categoryDetailsDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> DeleteCategory(int id)
    {
        var category = await _context.Category
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category is null) return NotFound($"Category with id {id} not found.");
        if (category.Products!.Count != 0) return BadRequest("Cannot delete category with associated products.");

        _context.Category.Remove(category);
        await _context.SaveChangesAsync();

        // Cria o DTO usando o construtor
        var categoryDetailsDto = new CategoryDetailsDto(category);

        return Ok(categoryDetailsDto);
    }
}