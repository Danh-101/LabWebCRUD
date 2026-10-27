using System.ComponentModel.DataAnnotations;

namespace LabWebCrud.Api.Models;

// Entity: 1 doi tuong Product tuong ung 1 dong trong bang Products.
public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Don vi VND, dung int de tranh sai so so thap phan khi hoc SQLite.
    [Range(0, int.MaxValue)]
    public int Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
