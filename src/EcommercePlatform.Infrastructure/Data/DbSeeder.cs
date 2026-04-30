using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await db.Database.MigrateAsync();

        string[] roles = { "Admin", "SocialStaff", "InventoryStaff" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await CreateUserAsync(userManager, "admin@ecommerce.local", "Admin@123456", "Admin");
        await CreateUserAsync(userManager, "social@ecommerce.local", "Staff@123456", "SocialStaff");
        await CreateUserAsync(userManager, "inventory@ecommerce.local", "Inventory@123456", "InventoryStaff");

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { Name = "Classic T-Shirt", Description = "Comfortable cotton t-shirt", SKU = "TSH-001", Price = 29.99m, PrimaryImageUrl = "https://via.placeholder.com/400x400?text=T-Shirt", IsActive = true, OnHand = 100 },
                new Product { Name = "Slim Jeans", Description = "Modern slim fit jeans", SKU = "JNS-001", Price = 79.99m, PrimaryImageUrl = "https://via.placeholder.com/400x400?text=Jeans", IsActive = true, OnHand = 50 },
                new Product { Name = "Running Shoes", Description = "Lightweight running shoes", SKU = "SHO-001", Price = 119.99m, PrimaryImageUrl = "https://via.placeholder.com/400x400?text=Shoes", IsActive = true, OnHand = 75 },
                new Product { Name = "Canvas Backpack", Description = "Durable canvas backpack", SKU = "BAG-001", Price = 49.99m, PrimaryImageUrl = "https://via.placeholder.com/400x400?text=Backpack", IsActive = true, OnHand = 30 },
                new Product { Name = "Leather Wallet", Description = "Genuine leather bifold wallet", SKU = "WAL-001", Price = 39.99m, PrimaryImageUrl = "https://via.placeholder.com/400x400?text=Wallet", IsActive = true, OnHand = 60 }
            );
            await db.SaveChangesAsync();
        }
    }

    private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
