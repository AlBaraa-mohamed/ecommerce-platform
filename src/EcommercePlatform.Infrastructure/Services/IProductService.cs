using EcommercePlatform.Domain.Entities;

namespace EcommercePlatform.Infrastructure.Services;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync(bool activeOnly = false);
    Task<Product?> GetProductAsync(int id);
    Task<Product?> GetProductBySkuAsync(string sku);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
