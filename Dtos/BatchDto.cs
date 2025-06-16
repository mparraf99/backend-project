namespace InventoryApi.DTOs;

public class BatchDto
{
    public int Id { get; set; }
    public string LotNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public decimal Price { get; set; }

    public decimal Quantity { get; set; }
}
