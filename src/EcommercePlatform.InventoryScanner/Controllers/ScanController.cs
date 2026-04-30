using EcommercePlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommercePlatform.InventoryScanner.Controllers;

[Authorize(Roles = "Admin,InventoryStaff")]
public class ScanController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IOrderFulfillmentService _fulfillmentService;

    public ScanController(IOrderService orderService, IOrderFulfillmentService fulfillmentService)
    {
        _orderService = orderService;
        _fulfillmentService = fulfillmentService;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Order(string orderNumber)
    {
        var order = await _orderService.GetOrderByNumberAsync(orderNumber);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDelivery(int orderId, int[] lineItemIds, int[] deliveredQuantities)
    {
        var deliveries = new Dictionary<int, int>();
        for (int i = 0; i < lineItemIds.Length; i++)
            deliveries[lineItemIds[i]] = deliveredQuantities[i];
        var userId = User.Identity?.Name ?? "";
        await _fulfillmentService.UpdateDeliveryAsync(orderId, deliveries, userId);
        TempData["Success"] = "Delivery updated.";
        return RedirectToAction("Order", new { orderNumber = (await _orderService.GetOrderAsync(orderId))?.OrderNumber });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReturn(int orderId, int[] lineItemIds, int[] returnedQuantities)
    {
        var returns = new Dictionary<int, int>();
        for (int i = 0; i < lineItemIds.Length; i++)
            returns[lineItemIds[i]] = returnedQuantities[i];
        var userId = User.Identity?.Name ?? "";
        await _fulfillmentService.UpdateReturnAsync(orderId, returns, userId);
        TempData["Success"] = "Return updated.";
        return RedirectToAction("Order", new { orderNumber = (await _orderService.GetOrderAsync(orderId))?.OrderNumber });
    }
}
