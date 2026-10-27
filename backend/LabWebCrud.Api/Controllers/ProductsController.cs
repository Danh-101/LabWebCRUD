using LabWebCrud.Api.Data;
using LabWebCrud.Api.DTOs;
using LabWebCrud.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabWebCrud.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(AppDbContext db) : ControllerBase
{
    // GET /api/products - Lay tat ca san pham.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        return Ok(products);
    }

    // GET /api/products/1 - Lay san pham theo ID.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound(new { message = "Khong tim thay san pham." }) : Ok(product);
    }

    // POST /api/products - Tao san pham moi.
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            Quantity = request.Quantity
        };
        if (string.IsNullOrWhiteSpace(product.Name))
            return BadRequest(new { message = "Ten san pham khong duoc de trong." });

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/1 - Sua thong tin san pham.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = "Khong tim thay san pham." });

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Ten san pham khong duoc de trong." });

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/products/1 - Xoa san pham.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = "Khong tim thay san pham." });

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
