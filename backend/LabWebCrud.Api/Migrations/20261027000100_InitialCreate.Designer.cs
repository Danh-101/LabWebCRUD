using LabWebCrud.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabWebCrud.Api.Migrations;

public partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");

        modelBuilder.Entity("LabWebCrud.Api.Models.Product", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .HasMaxLength(500)
                .HasColumnType("TEXT");

            b.Property<int>("Price").HasColumnType("INTEGER");
            b.Property<int>("Quantity").HasColumnType("INTEGER");

            b.HasKey("Id");
            b.ToTable("Products");

            b.HasData(
                new { Id = 1, Name = "Ban phim co", Description = "Ban phim co co den RGB", Price = 690000, Quantity = 12 },
                new { Id = 2, Name = "Chuot khong day", Description = "Chuot Bluetooth va USB", Price = 350000, Quantity = 20 },
                new { Id = 3, Name = "Tai nghe", Description = "Tai nghe chup tai", Price = 450000, Quantity = 8 }
            );
        });
#pragma warning restore 612, 618
    }
}
