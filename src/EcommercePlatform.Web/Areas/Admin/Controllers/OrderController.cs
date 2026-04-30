using ClosedXML.Excel;
using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace EcommercePlatform.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SocialStaff")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService) => _orderService = orderService;

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetOrdersAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status, string? notes)
    {
        var userId = User.Identity?.Name ?? "";
        await _orderService.UpdateOrderStatusAsync(id, status, userId, notes);
        TempData["Success"] = "Order status updated.";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkStatusUpdateRequest request)
    {
        if (request.OrderIds == null || !request.OrderIds.Any())
        {
            return BadRequest("No orders selected");
        }

        if (!Enum.TryParse<OrderStatus>(request.NewStatus, out var status))
        {
            return BadRequest("Invalid status");
        }

        var userId = User.Identity?.Name ?? "";
        foreach (var orderId in request.OrderIds)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, status, userId, $"Bulk update to {status}");
        }

        return Ok(new { message = $"Updated {request.OrderIds.Count} order(s)" });
    }

    public async Task<IActionResult> Label(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();

        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(order.OrderNumber, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);
        ViewBag.QRCodeBase64 = Convert.ToBase64String(qrBytes);

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> BulkPrintLabels([FromBody] BulkPrintRequest request)
    {
        if (request.OrderIds == null || !request.OrderIds.Any())
        {
            return BadRequest("No orders selected");
        }

        var orders = new List<Order>();
        var qrCodes = new Dictionary<int, string>();

        foreach (var orderId in request.OrderIds)
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order != null)
            {
                orders.Add(order);

                // Generate QR code for each order
                using var qrGenerator = new QRCodeGenerator();
                var qrData = qrGenerator.CreateQrCode(order.OrderNumber, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrData);
                var qrBytes = qrCode.GetGraphic(20);
                qrCodes[order.Id] = Convert.ToBase64String(qrBytes);
            }
        }

        ViewBag.QRCodes = qrCodes;
        return View("BulkLabels", orders);
    }

    public async Task<IActionResult> ExportExcel()
    {
        var orders = await _orderService.GetOrdersAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Orders");
        ws.Cell(1, 1).Value = "OrderNumber";
        ws.Cell(1, 2).Value = "CustomerName";
        ws.Cell(1, 3).Value = "CustomerPhone";
        ws.Cell(1, 4).Value = "ShippingAddress";
        ws.Cell(1, 5).Value = "ProductName";
        ws.Cell(1, 6).Value = "SKU";
        ws.Cell(1, 7).Value = "OrderedQty";
        ws.Cell(1, 8).Value = "QRCodePayload";

        int row = 2;
        foreach (var order in orders)
        {
            foreach (var line in order.LineItems)
            {
                ws.Cell(row, 1).Value = order.OrderNumber;
                ws.Cell(row, 2).Value = order.CustomerName;
                ws.Cell(row, 3).Value = order.CustomerPhone;
                ws.Cell(row, 4).Value = $"{order.ShippingAddress}, {order.ShippingCity}";
                ws.Cell(row, 5).Value = line.ProductNameSnapshot;
                ws.Cell(row, 6).Value = line.SKUSnapshot;
                ws.Cell(row, 7).Value = line.OrderedQuantity;
                ws.Cell(row, 8).Value = order.OrderNumber;
                row++;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SocialStaff")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin,SocialStaff")]
    public async Task<IActionResult> Create(Order order, int[] productIds, int[] quantities)
    {
        var lineItems = new List<OrderLineItem>();
        for (int i = 0; i < productIds.Length; i++)
        {
            if (quantities[i] > 0)
            {
                lineItems.Add(new OrderLineItem
                {
                    ProductId = productIds[i],
                    OrderedQuantity = quantities[i]
                });
            }
        }
        order.LineItems = lineItems;
        var userId = User.Identity?.Name ?? "";
        await _orderService.CreateOrderAsync(order, userId);
        TempData["Success"] = "Order created.";
        return RedirectToAction("Index");
    }
}

public class BulkStatusUpdateRequest
{
    public List<int> OrderIds { get; set; } = new();
    public string NewStatus { get; set; } = string.Empty;
}

public class BulkPrintRequest
{
    public List<int> OrderIds { get; set; } = new();
}
