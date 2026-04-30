using EcommercePlatform.Domain.Entities;

namespace EcommercePlatform.Infrastructure.Services;

public interface IInventoryService
{
    Task<StockMovement> AddMovementAsync(StockMovement movement);
    Task<List<StockMovement>> GetMovementsAsync(int? productId = null);
    Task UpdateStockAsync(int productId, int onHandDelta, int reservedDelta);
}
