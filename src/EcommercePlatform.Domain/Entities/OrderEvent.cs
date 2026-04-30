namespace EcommercePlatform.Domain.Entities;

public class OrderEvent
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public Order Order { get; set; } = null!;
}
