# EcommercePlatform

A full MVP e-commerce platform built with .NET 8, ASP.NET Core MVC, Entity Framework Core, and Bootstrap 5.

## Projects

| Project | Description |
|---------|-------------|
| `EcommercePlatform.Domain` | Domain entities |
| `EcommercePlatform.Infrastructure` | EF Core DbContext, services |
| `EcommercePlatform.Web` | Main storefront + admin panel |
| `EcommercePlatform.InventoryScanner` | Mobile QR scanner for delivery/returns |

   
## Setup

### Prerequisites
- .NET 8 SDK
- SQL Server or LocalDB

### Getting Started

```bash
# Clone and build
cd src/EcommercePlatform.Web
dotnet run
```

The app will apply EF migrations and seed the database automatically on first run.

### Default Accounts

| Email | Password | Role |
|-------|----------|------|
| admin@ecommerce.local | Admin@123456 | Admin |
| social@ecommerce.local | Staff@123456 | SocialStaff |
| inventory@ecommerce.local | Inventory@123456 | InventoryStaff |

### Connection String

Update `appsettings.json` in `EcommercePlatform.Web` and `EcommercePlatform.InventoryScanner`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=EcommercePlatformDb;..."
  }
}
```

## Features

- **Storefront**: Product catalog, cart, checkout
- **Admin Panel**: Dashboard, order management, product CRUD, inventory tracking
- **Inventory Scanner**: Mobile-friendly QR code scanner for delivery/return updates
- **Roles**: Admin, SocialStaff, InventoryStaff
- **Export**: Excel export for orders
- **Labels**: Printable shipping labels with QR codes
