using APICatalog.Models;
using APICatalog.Models.Dtos;
using APICatalog.Repositories.Sync;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Sync;

[ApiController]
[Route("api/v1/sync/[controller]")]
public class CategoryControllerSync : ControllerBase
{
    private readonly ICategoryRepositorySync _categoryRepository;
    private readonly ILogger<CategoryControllerSync> _logger;

    public CategoryControllerSync(ICategoryRepositorySync categoryRepository, ILogger<CategoryControllerSync> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoryDetailsDto>> GetCategories(int page = 1, int pageSize = 10)
    {
        var categories = _categoryRepository.GetAll(page, pageSize);
        var categoriesDto = categories.Select(c => new CategoryDetailsDto(c));
        return Ok(categoriesDto);
    }

    [HttpGet("{id:int}", Name = "GetCategoryByIdSync")]
    public ActionResult<CategoryDetailsDto> GetById(int id)
    {
        var category = _categoryRepository.GetById(id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria com ID {id} não encontrada.");
            return NotFound($"Categoria com ID {id} não encontrada.");
        }

        var categoryDto = new CategoryDetailsDto(category);
        return Ok(categoryDto);
    }

    [HttpPost]
    public ActionResult<CategoryDetailsDto> Create(CategoryDto? categoryDto)
    {
        if (categoryDto == null)
        {
            _logger.LogWarning("Requisição para criar categoria recebida com dados inválidos.");
            return BadRequest("Dados da categoria inválidos.");
        }

        var category = new Category
        {
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl
        };

        var createdCategory = _categoryRepository.Create(category);
        var categoryDetailsDto = new CategoryDetailsDto(createdCategory);

        return CreatedAtRoute("GetCategoryByIdSync", new { id = createdCategory.CategoryId }, categoryDetailsDto);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoryDetailsDto> Update(int id, CategoryDto? categoryDto)
    {
        if (categoryDto == null)
        {
            _logger.LogWarning($"Requisição para atualizar categoria com ID {id} recebida com dados inválidos.");
            return BadRequest("Dados da categoria inválidos.");
        }

        var category = new Category
        {
            CategoryId = id,
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl
        };

        var updatedCategory = _categoryRepository.Update(category);
        var categoryDetailsDto = new CategoryDetailsDto(updatedCategory);

        return Ok(categoryDetailsDto);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _categoryRepository.Delete(id);
        return NoContent();
    }
}