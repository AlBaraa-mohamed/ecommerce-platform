namespace EcommercePlatform.Domain.Entities;

public class OrderLineItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string SKUSnapshot { get; set; } = string.Empty;
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int OrderedQuantity { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
