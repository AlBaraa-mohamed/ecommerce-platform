using EcommercePlatform.Domain.Entities;

namespace EcommercePlatform.Infrastructure.Services;

public interface IOrderFulfillmentService
{
    Task UpdateDeliveryAsync(int orderId, Dictionary<int, int> lineItemDeliveries, string userId);
    Task UpdateReturnAsync(int orderId, Dictionary<int, int> lineItemReturns, string userId);
}
