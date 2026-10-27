using LabWebCrud.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LabWebCrud.Api.Data;

// DbContext la cau noi giua C# Entity va co so du lieu SQLite.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Du lieu mau duoc dua vao database trong migration dau tien.
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Ban phim co", Description = "Ban phim co co den RGB", Price = 690000, Quantity = 12 },
            new Product { Id = 2, Name = "Chuot khong day", Description = "Chuot Bluetooth va USB", Price = 350000, Quantity = 20 },
            new Product { Id = 3, Name = "Tai nghe", Description = "Tai nghe chup tai", Price = 450000, Quantity = 8 }
        );
    }
}
