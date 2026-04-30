namespace EcommercePlatform.Domain.Entities;

public enum OrderStatus
{
    New,
    Processing,
    PartiallyDelivered,
    Delivered,
    PartiallyReturned,
    Returned,
    Cancelled
}
