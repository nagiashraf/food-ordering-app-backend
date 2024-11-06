using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace FoodOrderingApp.Models;

[Collection("users")]
[Index(nameof(Email), IsUnique = true)]
public class User
{
    public ObjectId Id { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MinLength(6)]
    public string? Password { get; set; }

    public string? Name { get; set; }

    public string? AddressLine1 { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }
}