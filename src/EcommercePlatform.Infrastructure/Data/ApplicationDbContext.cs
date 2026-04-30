using EcommercePlatform.Domain.Entities;
using EcommercePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommercePlatform.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLineItem> OrderLineItems => Set<OrderLineItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.SKU).IsUnique();
            e.Ignore(p => p.Available);
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasIndex(o => o.OrderNumber).IsUnique();
        });

        modelBuilder.Entity<OrderLineItem>(e =>
        {
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrderLineItem_DeliveredQty", "[DeliveredQuantity] <= [OrderedQuantity]");
                t.HasCheckConstraint("CK_OrderLineItem_ReturnedQty", "[ReturnedQuantity] <= [DeliveredQuantity]");
            });
            e.Property(l => l.UnitPrice).HasColumnType("decimal(18,2)");
        });
    }
}
