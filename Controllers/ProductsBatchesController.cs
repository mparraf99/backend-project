using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/products-batches")]
[Authorize]
public class ProductBatchesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductBatchesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ProductBatches
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductBatch>>> GetAll()
    {
        return await _context.ProductBatches
            .Include(pb => pb.Product)
            .ToListAsync();
    }

    // GET: api/ProductBatches/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductBatch>> GetById(int id)
    {
        var batch = await _context.ProductBatches
            .Include(pb => pb.Product)
            .FirstOrDefaultAsync(pb => pb.Id == id);

        if (batch == null)
            return NotFound();

        return batch;
    }

    // POST: api/ProductBatches
    [HttpPost]
    public async Task<ActionResult<ProductBatch>> Create(ProductBatch batch)
    {
        _context.ProductBatches.Add(batch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = batch.Id }, batch);
    }

    // PUT: api/ProductBatches/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductBatch updatedBatch)
    {
        if (id != updatedBatch.Id)
            return BadRequest();

        var existingBatch = await _context.ProductBatches.FindAsync(id);
        if (existingBatch == null)
            return NotFound();

        existingBatch.LotNumber = updatedBatch.LotNumber;
        existingBatch.EntryDate = updatedBatch.EntryDate;
        existingBatch.Price = updatedBatch.Price;
        existingBatch.ProductId = updatedBatch.ProductId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/ProductBatches/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var batch = await _context.ProductBatches.FindAsync(id);
        if (batch == null)
            return NotFound();

        _context.ProductBatches.Remove(batch);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
