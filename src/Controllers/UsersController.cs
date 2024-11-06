using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodOrderingApp.Auth;
using FoodOrderingApp.Data;
using FoodOrderingApp.DTOs;
using FoodOrderingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace FoodOrderingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public UsersController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == new ObjectId(userId));

        if (user is null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthDto user)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (existingUser is not null)
        {
            return BadRequest("User already exists");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var newUser = new User { Email = user.Email, Password = user.Password };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        var jwtSecurityToken = _authService.CreateJwtToken(newUser);

        _authService.SetTokensInsideCookie(HttpContext, jwtSecurityToken);

        return Ok();
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update(UpdateUserDto userDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == new ObjectId(userId));

        if (user is null)
        {
            return NotFound("User not found");
        }

        user.Name = userDto.Name;
        user.AddressLine1 = userDto.AddressLine1;
        user.City = userDto.City;
        user.Country = userDto.Country;

        await _context.SaveChangesAsync();

        return Ok(user);
    }
}