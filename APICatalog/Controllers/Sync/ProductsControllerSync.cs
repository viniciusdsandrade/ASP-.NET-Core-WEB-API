using APICatalog.Models;
using APICatalog.Repositories.Sync;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Sync;

[ApiController]
[Route("api/v1/sync/[controller]")]
public class ProductsControllerSync : ControllerBase
{
    private readonly IProductRepositorySync _repository;

    public ProductsControllerSync(IProductRepositorySync repository) => _repository = repository;

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = _repository.GetAll();
        if (products is null) return NotFound("No products found.");
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> Get(int id)
    {
        var product = _repository.GetById(id);
        if (product is null) return NotFound($"Product with id {id} not found.");
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Post(Product product)
    {
        if (product is null) return BadRequest("Product data is null.");
        var newProduct = _repository.Create(product);
        return CreatedAtAction(nameof(Get), new { id = newProduct.ProductId }, newProduct);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Product product)
    {
        if (id != product.ProductId) return BadRequest("Product id mismatch.");
        var atualizado = _repository.Update(product);
        if (!atualizado) return NotFound($"Product with id {id} not found.");
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deletado = _repository.Delete(id);
        if (!deletado) return NotFound($"Product with id {id} not found.");
        return NoContent();
    }
}