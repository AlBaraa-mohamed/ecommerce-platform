using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Services;

public class OrderFulfillmentService : IOrderFulfillmentService
{
    private readonly ApplicationDbContext _db;
    private readonly IInventoryService _inventory;
    private readonly IOrderService _orderService;

    public OrderFulfillmentService(ApplicationDbContext db, IInventoryService inventory, IOrderService orderService)
    {
        _db = db;
        _inventory = inventory;
        _orderService = orderService;
    }

    public async Task UpdateDeliveryAsync(int orderId, Dictionary<int, int> lineItemDeliveries, string userId)
    {
        var order = await _db.Orders.Include(o => o.LineItems).FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new InvalidOperationException("Order not found");

        foreach (var (lineItemId, deliveredQty) in lineItemDeliveries)
        {
            var line = order.LineItems.FirstOrDefault(l => l.Id == lineItemId);
            if (line == null) continue;

            var additionalDelivered = deliveredQty - line.DeliveredQuantity;
            if (additionalDelivered <= 0) continue;

            line.DeliveredQuantity = deliveredQty;

            await _inventory.AddMovementAsync(new StockMovement
            {
                ProductId = line.ProductId,
                MovementType = MovementType.Deliver,
                Quantity = additionalDelivered,
                OrderId = orderId,
                OrderLineItemId = lineItemId,
                Notes = $"Delivered for order {order.OrderNumber}",
                CreatedByUserId = userId
            });
            await _inventory.UpdateStockAsync(line.ProductId, -additionalDelivered, -additionalDelivered);
        }

        bool allDelivered = order.LineItems.All(l => l.DeliveredQuantity == l.OrderedQuantity);
        bool anyDelivered = order.LineItems.Any(l => l.DeliveredQuantity > 0);

        if (allDelivered)
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered, userId, "All items delivered");
        else if (anyDelivered)
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.PartiallyDelivered, userId, "Partial delivery");

        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateReturnAsync(int orderId, Dictionary<int, int> lineItemReturns, string userId)
    {
        var order = await _db.Orders.Include(o => o.LineItems).FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new InvalidOperationException("Order not found");

        foreach (var (lineItemId, returnedQty) in lineItemReturns)
        {
            var line = order.LineItems.FirstOrDefault(l => l.Id == lineItemId);
            if (line == null) continue;

            var additionalReturned = returnedQty - line.ReturnedQuantity;
            if (additionalReturned <= 0) continue;

            line.ReturnedQuantity = returnedQty;

            await _inventory.AddMovementAsync(new StockMovement
            {
                ProductId = line.ProductId,
                MovementType = MovementType.Return,
                Quantity = additionalReturned,
                OrderId = orderId,
                OrderLineItemId = lineItemId,
                Notes = $"Returned from order {order.OrderNumber}",
                CreatedByUserId = userId
            });
            await _inventory.UpdateStockAsync(line.ProductId, additionalReturned, 0);
        }

        bool allReturned = order.LineItems.All(l => l.ReturnedQuantity == l.DeliveredQuantity);
        bool anyReturned = order.LineItems.Any(l => l.ReturnedQuantity > 0);

        if (allReturned)
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Returned, userId, "All items returned");
        else if (anyReturned)
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.PartiallyReturned, userId, "Partial return");

        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
