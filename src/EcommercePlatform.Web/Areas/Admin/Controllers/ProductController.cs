using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommercePlatform.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;

    public ProductController(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync();
        return View(products);
    }

    public IActionResult Create() => View(new Product());

    [HttpPost]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        if (imageFile != null)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);
            product.PrimaryImageUrl = $"/uploads/{fileName}";
        }
        ModelState.Clear();
        await _productService.CreateProductAsync(product);
        TempData["Success"] = "Product created.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
    {
        var existing = await _productService.GetProductAsync(product.Id);
        if (existing == null) return NotFound();

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.SKU = product.SKU;
        existing.Price = product.Price;
        existing.IsActive = product.IsActive;

        if (imageFile != null)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);
            existing.PrimaryImageUrl = $"/uploads/{fileName}";
        }

        ModelState.Clear();
        await _productService.UpdateProductAsync(existing);
        TempData["Success"] = "Product updated.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        TempData["Success"] = "Product deleted.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }
}
