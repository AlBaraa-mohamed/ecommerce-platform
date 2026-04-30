using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Services;
using EcommercePlatform.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommercePlatform.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService productService, IOrderService orderService, ILogger<HomeController> logger)
    {
        _productService = productService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync(activeOnly: true);
        return View(products);
    }

    public async Task<IActionResult> Product(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null || !product.IsActive) return NotFound();
        return View(product);
    }

    public IActionResult Cart()
    {
        var cart = GetCart();
        return View(cart);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);
        if (existing != null)
            existing.Quantity += quantity;
        else
            cart.Add(new CartItem { ProductId = productId, Quantity = quantity });
        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.ProductId == productId);
        SaveCart(cart);
        return RedirectToAction("Cart");
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (!cart.Any()) return RedirectToAction("Cart");
        var products = await _productService.GetProductsAsync(activeOnly: true);
        ViewBag.Products = products.Where(p => cart.Select(c => c.ProductId).Contains(p.Id)).ToList();
        ViewBag.Cart = cart;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(string customerName, string customerPhone, string customerEmail,
        string shippingAddress, string shippingCity, string shippingState, string shippingPostalCode, string shippingCountry)
    {
        var cart = GetCart();
        if (!cart.Any()) return RedirectToAction("Cart");

        var products = await _productService.GetProductsAsync(activeOnly: true);
        var lineItems = new List<OrderLineItem>();

        foreach (var item in cart)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) continue;
            lineItems.Add(new OrderLineItem
            {
                ProductId = product.Id,
                SKUSnapshot = product.SKU,
                ProductNameSnapshot = product.Name,
                UnitPrice = product.Price,
                OrderedQuantity = item.Quantity
            });
        }

        var order = new Order
        {
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail ?? string.Empty,
            ShippingAddress = shippingAddress,
            ShippingCity = shippingCity,
            ShippingState = shippingState ?? string.Empty,
            ShippingPostalCode = shippingPostalCode ?? string.Empty,
            ShippingCountry = shippingCountry,
            LineItems = lineItems
        };

        var userId = User.Identity?.Name ?? "guest";
        await _orderService.CreateOrderAsync(order, userId);
        SaveCart(new List<CartItem>());

        return RedirectToAction("OrderConfirmation", new { orderNumber = order.OrderNumber });
    }

    public async Task<IActionResult> OrderConfirmation(string orderNumber)
    {
        var order = await _orderService.GetOrderByNumberAsync(orderNumber);
        if (order == null) return NotFound();
        return View(order);
    }

    public IActionResult Error() => View();

    private List<CartItem> GetCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        return cartJson != null ? JsonSerializer.Deserialize<List<CartItem>>(cartJson)! : new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
    }
}
