﻿using APICatalog.Models;
using APICatalog.Models.Dtos;
using APICatalog.Repositories.Async;
using APICatalog.Repositories.Generic;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Async;

[ApiController]
[Route("api/v1/async/[controller]")]
public class CategoryControllerAsync(
    IRepositoryAsync<Category> repositoryAsyncGeneric,
    ICategoryRepositoryAsync repository,
    ILogger<CategoryControllerAsync> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CategoryDetailsDto>> CreateCategory(CategoryDto? categoryDto)
    {
        logger.LogInformation("Criando nova categoria.");

        if (categoryDto is null)
        {
            logger.LogWarning("Requisição para criar categoria recebida com dados inválidos.");
            return BadRequest("Category data is null.");
        }

        var category = new Category
        {
            Name = categoryDto.Name,
            ImageUrl = categoryDto.ImageUrl
        };

        await repository.CreateAsync(category);
        await repository.SaveChangesAsync();

        var categoryDetailsDto = new CategoryDetailsDto(category);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, categoryDetailsDto);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<ActionResult<CategoryDetailsDto>> GetCategoryById(int id)
    {
        logger.LogInformation("Obtendo categoria por ID: {id}", id);

        var category = await repositoryAsyncGeneric.GetByIdAsync(id);

        if (category is null)
        {
            logger.LogWarning("Categoria com ID {id} não encontrada.", id);
            return NotFound($"Category with id {id} not found.");
        }

        var categoryDetailsDto = new CategoryDetailsDto(category);

        return Ok(categoryDetailsDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDetailsDto>>> GetCategories(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        logger.LogInformation("Obtendo categorias. Página: {page}, Tamanho da página: {pageSize}", page, pageSize);

        if (page <= 0) return BadRequest("Page must be greater than zero.");
        if (pageSize is <= 0 or > 100) return BadRequest("Page size must be between 1 and 100.");

        var categories = await repository.GetAllAsync(page, pageSize);

        var categoriesList = categories.ToList();

        if (categoriesList.Count == 0)
        {
            logger.LogInformation("Nenhuma categoria encontrada para a página {page}.", page);
            return NotFound("No categories found.");
        }

        var categoriesDetailsDto = categoriesList.Select(c => new CategoryDetailsDto(c));

        return Ok(categoriesDetailsDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> UpdateCategory(int id, CategoryDto? categoryDto)
    {
        logger.LogInformation("Atualizando categoria com ID {id}.", id);

        if (categoryDto is null)
        {
            logger.LogWarning("Requisição para atualizar categoria com ID {id} recebida com dados inválidos.", id);
            return BadRequest("Category data is null.");
        }

        var existingCategory = await repository.GetByIdAsync(id);

        if (existingCategory is null)
        {
            logger.LogWarning("Categoria com ID {id} não encontrada para atualização.", id);
            return NotFound($"Category with id {id} not found.");
        }

        existingCategory.Name = categoryDto.Name;
        existingCategory.ImageUrl = categoryDto.ImageUrl;

        await repository.UpdateAsync(existingCategory);
        await repository.SaveChangesAsync();

        var categoryDetailsDto = new CategoryDetailsDto(existingCategory);

        return CreatedAtAction(nameof(GetCategoryById), new { id = existingCategory.CategoryId }, categoryDetailsDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoryDetailsDto>> DeleteCategory(int id)
    {
        logger.LogInformation("Excluindo categoria com ID {id}.", id);

        var category = await repository.GetByIdAsync(id);

        if (category is null)
        {
            logger.LogWarning("Categoria com ID {id} não encontrada para exclusão.", id);
            return NotFound($"Category with id {id} not found.");
        }

        if (category.Products != null && category.Products.Count != 0)
        {
            logger.LogWarning("Não é possível excluir a categoria com ID {id} pois possui produtos associados.", id);
            return BadRequest("Cannot delete category with associated products.");
        }

        await repository.DeleteAsync(id);
        await repository.SaveChangesAsync();

        var categoryDetailsDto = new CategoryDetailsDto(category);

        return Ok(categoryDetailsDto);
    }
}