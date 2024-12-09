using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace FoodOrderingApp.Models;

[Collection("restaurants")]
public class Restaurant
{
    public ObjectId Id { get; set; }
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? City { get; set; }

    [Required]
    public string? Country { get; set; }

    [Required]
    public decimal DeliveryPrice { get; set; }

    [Required]
    public int EstimatedDeliveryTimeInMinutes { get; set; }

    [Required]
    public List<string> Cuisines { get; set; } = [];

    [Required]
    public string? ImageUrl { get; set; }

    [Required]
    public DateTime LastUpdated { get; set; }

    public ObjectId UserId { get; set; } // Foreign keys and navigation traversal are not supported yet but considered for future releases: https://github.com/mongodb/mongo-efcore-provider?tab=readme-ov-file#not-supported-but-considering-for-future-releases

    [Required]
    public List<MenuItem> MenuItems { get; set; } = [];
}

public class MenuItem
{
    // public ObjectId Id { get; set; }

    [Required]
    public string? Name { get; set;}

    [Required]
    public decimal Price { get; set; }

    // public ObjectId RestaurantId { get; set; } 
}