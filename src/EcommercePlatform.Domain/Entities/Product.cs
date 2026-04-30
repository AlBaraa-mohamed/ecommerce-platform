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
    public ProductType Type { get; set; } = ProductType.General;
    public int? Size { get; set; } // For shoes: 38-45
}

public enum ProductType
{
    General = 0,
    Clothing = 1,
    Shoes = 2,
    Electronics = 3,
    Accessories = 4,
    Books = 5
}
