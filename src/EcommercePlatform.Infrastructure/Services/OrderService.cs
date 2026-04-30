using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _db;
    private readonly IInventoryService _inventory;

    public OrderService(ApplicationDbContext db, IInventoryService inventory)
    {
        _db = db;
        _inventory = inventory;
    }

    public async Task<Order> CreateOrderAsync(Order order, string userId)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.CreatedByUserId = userId;
        order.Status = OrderStatus.New;

        var count = await _db.Orders.CountAsync();
        order.OrderNumber = $"ORD-{(count + 1):D5}";

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        foreach (var line in order.LineItems)
        {
            await _inventory.AddMovementAsync(new StockMovement
            {
                ProductId = line.ProductId,
                MovementType = MovementType.Reserve,
                Quantity = line.OrderedQuantity,
                OrderId = order.Id,
                OrderLineItemId = line.Id,
                Notes = $"Reserved for order {order.OrderNumber}",
                CreatedByUserId = userId
            });
            await _inventory.UpdateStockAsync(line.ProductId, 0, line.OrderedQuantity);
        }

        _db.OrderEvents.Add(new OrderEvent
        {
            OrderId = order.Id,
            EventType = "Created",
            Description = $"Order {order.OrderNumber} created",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        });
        await _db.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderAsync(int id) =>
        await _db.Orders
            .Include(o => o.LineItems).ThenInclude(l => l.Product)
            .Include(o => o.Events)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order?> GetOrderByNumberAsync(string orderNumber) =>
        await _db.Orders
            .Include(o => o.LineItems).ThenInclude(l => l.Product)
            .Include(o => o.Events)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

    public async Task<List<Order>> GetOrdersAsync() =>
        await _db.Orders
            .Include(o => o.LineItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status, string userId, string? notes = null)
    {
        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} not found");
        var oldStatus = order.Status;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        _db.OrderEvents.Add(new OrderEvent
        {
            OrderId = orderId,
            EventType = "StatusChanged",
            Description = $"Status changed from {oldStatus} to {status}. {notes}",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        });
        await _db.SaveChangesAsync();
    }
}
