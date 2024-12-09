using System.Security.Claims;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodOrderingApp.Data;
using FoodOrderingApp.DTOs;
using FoodOrderingApp.Helpers;
using FoodOrderingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace FoodOrderingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly Cloudinary _cloudinary;

    public RestaurantsController(AppDbContext context, IOptions<CloudinarySettings> cloudinarySettings)
    {
        _context = context;

        var account = new Account(
            cloudinarySettings.Value.CloudName,
            cloudinarySettings.Value.ApiKey,
            cloudinarySettings.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.UserId == new ObjectId(userId));

        if (restaurant is null)
        {
            return NotFound("Restaurant not found");
        }

        return Ok(restaurant);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] RestaurantDto restaurantDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var existingRestaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.UserId == new ObjectId(userId));

        if (existingRestaurant is not null)
        {
            return Conflict("User already has a restaurant.");
        }

        var imageUploadResult = await UploadImage(restaurantDto.ImageFile);

        if (imageUploadResult.Error is not null)
        {
            return BadRequest(imageUploadResult.Error.Message);
        }

        var restaurant = new Restaurant
        {
            UserId = new ObjectId(userId),
            Name = restaurantDto.Name,
            City = restaurantDto.City,
            Country = restaurantDto.Country,
            DeliveryPrice = restaurantDto.DeliveryPrice,
            EstimatedDeliveryTimeInMinutes = restaurantDto.EstimatedDeliveryTimeInMinutes,
            LastUpdated = DateTime.UtcNow,
            Cuisines = restaurantDto.Cuisines,
            MenuItems = restaurantDto.MenuItems
              .Select(m => new MenuItem { Name = m.Name, Price = m.Price })
              .ToList(),
            ImageUrl = imageUploadResult.SecureUrl.AbsoluteUri
        };

        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = restaurant.Id }, restaurant);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromForm] RestaurantDto restaurantDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.UserId == new ObjectId(userId));

        if (restaurant is null)
        {
            return NotFound("Restaurant not found");
        }

        restaurant.Name = restaurantDto.Name;
        restaurant.City = restaurantDto.City;
        restaurant.Country = restaurantDto.Country;
        restaurant.DeliveryPrice = restaurantDto.DeliveryPrice;
        restaurant.EstimatedDeliveryTimeInMinutes = restaurantDto.EstimatedDeliveryTimeInMinutes;
        restaurant.Cuisines = restaurantDto.Cuisines;
        restaurant.MenuItems = restaurantDto.MenuItems
          .Select(m => new MenuItem { Name = m.Name, Price = m.Price })
          .ToList();
        restaurant.LastUpdated = DateTime.UtcNow;

        if (restaurantDto.ImageFile.Length > 0)
        {
            var imageUploadResult = await UploadImage(restaurantDto.ImageFile);

            if (imageUploadResult.Error is not null)
            {
                return BadRequest(imageUploadResult.Error.Message);
            }

            restaurant.ImageUrl = imageUploadResult.SecureUrl.AbsoluteUri;
        }

        await _context.SaveChangesAsync();

        return Ok(restaurant);
    }

    private async Task<ImageUploadResult> UploadImage(IFormFile imageFile)
    {
        var imageUploadResult = new ImageUploadResult();

        if (imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageFile.FileName, stream),
            };
            imageUploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return imageUploadResult;
    }
}