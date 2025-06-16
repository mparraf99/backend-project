using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.DTOs;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    if (page <= 0 || pageSize <= 0)
        return BadRequest("Page and pageSize must be greater than 0.");

    var query = _context.Products
        .Include(p => p.Batches)
        .AsQueryable();

    var totalItems = await query.CountAsync();

    var products = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var productDtos = products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Batches = (p.Batches ?? new List<ProductBatch>()).Select(b => new BatchDto
        {
            Id = b.Id,
            LotNumber = b.LotNumber,
            EntryDate = b.EntryDate,
            Price = b.Price
        }).ToList()
    }).ToList();

    var response = new
    {
        TotalItems = totalItems,
        Page = page,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
        Items = productDtos
    };

    return Ok(response);
}


    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Batches)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Batches = (product.Batches ?? new List<ProductBatch>()).Select(b => new BatchDto
            {
                Id = b.Id,
                LotNumber = b.LotNumber,
                EntryDate = b.EntryDate,
                Price = b.Price
            }).ToList()
        };

        return Ok(productDto);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product updatedProduct)
    {
        if (id != updatedProduct.Id)
            return BadRequest();

        var product = await _context.Products
            .Include(p => p.Batches)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;

        var updatedBatches = updatedProduct.Batches ?? new List<ProductBatch>();
        var existingBatches = product.Batches?.ToList() ?? new List<ProductBatch>();

        foreach (var existingBatch in existingBatches)
        {
            if (!updatedBatches.Any(b => b.Id == existingBatch.Id))
            {
                _context.Remove(existingBatch);
            }
        }

        foreach (var updatedBatch in updatedBatches)
        {
            var existingBatch = existingBatches.FirstOrDefault(b => b.Id == updatedBatch.Id);

            if (existingBatch != null)
            {
                existingBatch.LotNumber = updatedBatch.LotNumber;
                existingBatch.EntryDate = updatedBatch.EntryDate;
                existingBatch.Price = updatedBatch.Price;
            }
            else
            {
                product.Batches!.Add(new ProductBatch
                {
                    LotNumber = updatedBatch.LotNumber,
                    EntryDate = updatedBatch.EntryDate,
                    Price = updatedBatch.Price,
                    ProductId = product.Id
                });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.Include(p => p.Batches).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
