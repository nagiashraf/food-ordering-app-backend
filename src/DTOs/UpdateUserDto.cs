using System.ComponentModel.DataAnnotations;

namespace FoodOrderingApp.DTOs;

public class UpdateUserDto
{
  [Required]
    public string? Name { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }
}