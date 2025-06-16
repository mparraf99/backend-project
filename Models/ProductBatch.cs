namespace InventoryApi.Models;

public class ProductBatch
{
    public int Id { get; set; }

    public string LotNumber { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }

    public decimal Price { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public Product? Product { get; set; }
}
