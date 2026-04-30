using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _db;

    public ProductService(ApplicationDbContext db) => _db = db;

    public async Task<List<Product>> GetProductsAsync(bool activeOnly = false)
    {
        var query = _db.Products.Include(p => p.Images).AsQueryable();
        if (activeOnly) query = query.Where(p => p.IsActive);
        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int id) =>
        await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product?> GetProductBySkuAsync(string sku) =>
        await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.SKU == sku);

    public async Task<Product> CreateProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }
    }
}
