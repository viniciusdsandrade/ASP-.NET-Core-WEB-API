using APICatalog.Context;
using APICatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalog.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<ActionResult<CategoryDetailsDto>> PostCategory(CategoryDto? categoryDto)
    {
        if (categoryDto is null) return BadRequest("Category data is null.");

        var category = new Category
        {
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl
        };

        await _context.Category.AddAsync(category);
        await _context.SaveChangesAsync();

        var categoryDetailsDto = new CategoryDetailsDto(
            category.CategoryId,
            category.Name!,
            category.ImageUrl!,
            [] // Inicializa a lista de produtos
        );

        return CreatedAtAction(nameof(GetCategoryDetails), new { id = category.CategoryId }, categoryDetailsDto);
    }

    [HttpGet("details/{id:int}", Name = "GetCategoryDetails")]
    public async Task<ActionResult<CategoryDetailsDto>> GetCategoryDetails(int id)
    {
        var category = await _context.Category
            .Include(c => c.Products) // Carregamos os produtos relacionados
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category is null) return NotFound($"Category with id {id} not found.");

        var categoryDetailsDto = new CategoryDetailsDto(
            category.CategoryId,
            category.Name!,
            category.ImageUrl!,
            category.Products?.Select(p => p.Name).ToList()! // Obtemos os nomes dos produtos
        );

        return Ok(categoryDetailsDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDetailsDto>>> GetCategories(int page = 1, int pageSize = 10)
    {
        var categories = await _context.Category
            .Include(c => c.Products) // Carregamos os produtos relacionados
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        if (categories.Count == 0) return NotFound("No categories found.");

        var categoriesDetailsDto = categories.Select(c => new CategoryDetailsDto(
            c.CategoryId,
            c.Name!,
            c.ImageUrl!,
            c.Products?.Select(p => p.Name).ToList()! // Obtemos os nomes dos produtos
        ));


        return Ok(categoriesDetailsDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> PutCategory(int id, Category? category)
    {
        if (category is null) return BadRequest("Category data is null.");

        var existingCategory = await _context.Category.FindAsync(id);

        if (existingCategory is null) return NotFound($"Category with id {id} not found.");

        // Atualiza as propriedades da categoria existente
        existingCategory.Name = category.Name;
        existingCategory.ImageUrl = category.ImageUrl;

        // Salva as alterações no banco de dados
        await _context.SaveChangesAsync();

        // Carrega os produtos relacionados APÓS a atualização
        await _context.Entry(existingCategory)
            .Collection(c => c.Products!)
            .LoadAsync();

        // Cria o DTO com os dados atualizados, incluindo os produtos
        var categoryDetailsDto = new CategoryDetailsDto(
            existingCategory.CategoryId,
            existingCategory.Name!,
            existingCategory.ImageUrl!,
            existingCategory.Products?.Select(p => p.Name).ToList()!
        );

        return CreatedAtAction(nameof(GetCategoryDetails), new { id = existingCategory.CategoryId },
            categoryDetailsDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> DeleteCategory(int id)
    {
        var category = await _context.Category
            .Include(c => c.Products) // Carrega os produtos relacionados
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category is null) return NotFound($"Category with id {id} not found.");

        if (category.Products!.Count != 0) return BadRequest("Cannot delete category with associated products.");

        _context.Category.Remove(category);
        await _context.SaveChangesAsync();

        var categoryDetailsDto = new CategoryDetailsDto(
            category.CategoryId,
            category.Name!,
            category.ImageUrl!,
            category.Products?.Select(p => p.Name).ToList()!
        );

        return Ok(categoryDetailsDto);
    }
}