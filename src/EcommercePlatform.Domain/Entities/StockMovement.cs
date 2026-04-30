namespace EcommercePlatform.Domain.Entities;

public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public MovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public int? OrderId { get; set; }
    public int? OrderLineItemId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public Product Product { get; set; } = null!;
}
