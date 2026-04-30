namespace EcommercePlatform.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
    public string ShippingPostalCode { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<OrderLineItem> LineItems { get; set; } = new();
    public List<OrderEvent> Events { get; set; } = new();
}
