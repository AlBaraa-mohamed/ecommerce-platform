using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _db;

    public InventoryService(ApplicationDbContext db) => _db = db;

    public async Task<StockMovement> AddMovementAsync(StockMovement movement)
    {
        movement.CreatedAt = DateTime.UtcNow;
        _db.StockMovements.Add(movement);
        await _db.SaveChangesAsync();
        return movement;
    }

    public async Task<List<StockMovement>> GetMovementsAsync(int? productId = null)
    {
        var query = _db.StockMovements.Include(m => m.Product).AsQueryable();
        if (productId.HasValue) query = query.Where(m => m.ProductId == productId.Value);
        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
    }

    public async Task UpdateStockAsync(int productId, int onHandDelta, int reservedDelta)
    {
        var product = await _db.Products.FindAsync(productId)
            ?? throw new InvalidOperationException($"Product {productId} not found");
        product.OnHand += onHandDelta;
        product.Reserved += reservedDelta;
        await _db.SaveChangesAsync();
    }
}
