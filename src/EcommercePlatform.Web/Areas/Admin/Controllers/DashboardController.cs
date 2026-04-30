using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,SocialStaff")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalOrders = await _db.Orders.CountAsync();
        ViewBag.NewOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.New);
        ViewBag.TotalProducts = await _db.Products.CountAsync(p => p.IsActive);
        ViewBag.TotalRevenue = await _db.OrderLineItems
            .Where(l => l.Order.Status != OrderStatus.Cancelled)
            .SumAsync(l => (decimal?)(l.UnitPrice * l.OrderedQuantity)) ?? 0m;
        ViewBag.RecentOrders = await _db.Orders
            .Include(o => o.LineItems)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();
        return View();
    }
}
