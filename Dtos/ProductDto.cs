using System.Collections.Generic;

namespace InventoryApi.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<BatchDto> Batches { get; set; } = new();
}
