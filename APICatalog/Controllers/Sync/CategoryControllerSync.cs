using APICatalog.Models;
using APICatalog.Models.Dtos;
using APICatalog.Repositories.UnitOfWork.Sync;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Sync;

[ApiController]
[Route("api/v1/sync/[controller]")]
public class CategoryControllerSync : ControllerBase
{
    private readonly IUnitOfWorkSync _uof;
    private readonly ILogger<CategoryControllerSync> _logger;

    public CategoryControllerSync(IUnitOfWorkSync uof,
        ILogger<CategoryControllerSync> logger)
    {
        _uof = uof;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoryDetailsDto>> GetCategories(int page = 1, int pageSize = 10)
    {
        var categories = _uof.CategoryRepositorySync.GetAll(page, pageSize);
        var categoriesDto = categories.Select(c => new CategoryDetailsDto(c));
        return Ok(categoriesDto);
    }

    [HttpGet("{id:int}", Name = "GetCategoryByIdSync")]
    public ActionResult<CategoryDetailsDto> GetById(int id)
    {
        var category = _uof.CategoryRepositorySync.GetById(id);
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

        var createdCategory = _uof.CategoryRepositorySync.Create(category);
        var categoryDetailsDto = new CategoryDetailsDto(createdCategory);

        return CreatedAtRoute("GetCategoryByIdSync", new { id = categoryDetailsDto.CategoryId }, categoryDetailsDto);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoryDetailsDto> Update(int id, CategoryDto? categoryDto)
    {
        if (categoryDto == null)
        {
            _logger.LogWarning("Requisição para atualizar categoria recebida com dados inválidos.");
            return BadRequest("Dados da categoria inválidos.");
        }

        var category = _uof.CategoryRepositorySync.GetById(id);

        category.Name = categoryDto.Name;
        category.ImageUrl = categoryDto.ImageUrl;

        var updatedCategory = _uof.CategoryRepositorySync.Update(category);
        var categoryDetailsDto = new CategoryDetailsDto(updatedCategory);

        return Ok(categoryDetailsDto);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _uof.CategoryRepositorySync.Delete(id);
        return NoContent();
    }
}