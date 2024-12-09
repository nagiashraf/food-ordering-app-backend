using System.ComponentModel.DataAnnotations;

namespace FoodOrderingApp.DTOs;

public class CreateMenuItemDto
{
    [Required]
    public string? Name { get; set;}

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Price must be positive")]
    public decimal Price { get; set; }
}