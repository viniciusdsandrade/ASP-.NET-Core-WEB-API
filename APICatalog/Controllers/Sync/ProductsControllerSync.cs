using APICatalog.Models;
using APICatalog.Repositories.Sync;
using Microsoft.AspNetCore.Mvc;

namespace APICatalog.Controllers.Sync;

[ApiController]
[Route("api/v1/sync/[controller]")]
public class ProductsControllerSync(IProductRepositorySync repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = repository.GetAll();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> Get(int id)
    {
        var product = repository.GetById(id);
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Post(Product product)
    {
        var newProduct = repository.Create(product);
        return CreatedAtAction(nameof(Get), new { id = newProduct.ProductId }, newProduct);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Product product)
    {
        if (id != product.ProductId) return BadRequest("Product id mismatch.");
        var atualizado = repository.Update(product);
        if (!atualizado) return NotFound($"Product with id {id} not found.");
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deletado = repository.Delete(id);
        if (!deletado) return NotFound($"Product with id {id} not found.");
        return NoContent();
    }
}