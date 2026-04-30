using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommercePlatform.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SocialStaff")]
public class InventoryController : Controller
{
    private readonly IProductService _productService;
    private readonly IInventoryService _inventoryService;

    public InventoryController(IProductService productService, IInventoryService inventoryService)
    {
        _productService = productService;
        _inventoryService = inventoryService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync();
        return View(products);
    }

    public async Task<IActionResult> Movements(int? productId = null)
    {
        var movements = await _inventoryService.GetMovementsAsync(productId);
        ViewBag.ProductId = productId;
        return View(movements);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Adjust(int productId, int quantity, string notes)
    {
        var userId = User.Identity?.Name ?? "";
        var movement = new StockMovement
        {
            ProductId = productId,
            MovementType = MovementType.Adjustment,
            Quantity = Math.Abs(quantity),
            Notes = notes,
            CreatedByUserId = userId
        };
        await _inventoryService.AddMovementAsync(movement);
        await _inventoryService.UpdateStockAsync(productId, quantity, 0);
        TempData["Success"] = "Inventory adjusted.";
        return RedirectToAction("Index");
    }
}
