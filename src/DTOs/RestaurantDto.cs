using System.ComponentModel.DataAnnotations;

namespace FoodOrderingApp.DTOs;

public class RestaurantDto
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? City { get; set; }

    [Required]
    public string? Country { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Delivery price must be positive")]
    public decimal DeliveryPrice { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Estimated delivery time must be positive")]
    public int EstimatedDeliveryTimeInMinutes { get; set; }

    [Required]
    public IFormFile ImageFile { get; set; } = null!;

    [MinLength(1)]
    public List<string> Cuisines { get; set; } = [];

    [MinLength(1)]
    public List<CreateMenuItemDto> MenuItems { get; set; } = [];
}