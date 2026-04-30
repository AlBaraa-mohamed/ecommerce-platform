using EcommercePlatform.Domain.Entities;

namespace EcommercePlatform.Infrastructure.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order, string userId);
    Task<Order?> GetOrderAsync(int id);
    Task<Order?> GetOrderByNumberAsync(string orderNumber);
    Task<List<Order>> GetOrdersAsync();
    Task UpdateOrderStatusAsync(int orderId, OrderStatus status, string userId, string? notes = null);
}
