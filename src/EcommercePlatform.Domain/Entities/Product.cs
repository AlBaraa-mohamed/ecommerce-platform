namespace EcommercePlatform.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PrimaryImageUrl { get; set; } = string.Empty;
    public List<ProductImage> Images { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available => OnHand - Reserved;
}
