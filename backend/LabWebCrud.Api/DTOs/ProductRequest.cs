using System.ComponentModel.DataAnnotations;

namespace LabWebCrud.Api.DTOs;

// DTO: du lieu cho phep Frontend gui len khi POST/PUT.
// Khong cho client tu gan Id.
public class ProductRequest
{
    [Required(ErrorMessage = "Ten san pham la bat buoc.")]
    [StringLength(120, ErrorMessage = "Ten khong duoc qua 120 ky tu.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mo ta khong duoc qua 500 ky tu.")]
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Gia phai lon hon hoac bang 0.")]
    public int Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "So luong phai lon hon hoac bang 0.")]
    public int Quantity { get; set; }
}
