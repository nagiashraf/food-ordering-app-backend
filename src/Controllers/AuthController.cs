using System.Security.Claims;
using FoodOrderingApp.Auth;
using FoodOrderingApp.Data;
using FoodOrderingApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthDto loginDto)
    {
        var user =await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user is null)
        {
            return BadRequest("Invalid credentials");
        }

        var isMatch = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

        if (!isMatch)
        {
            return BadRequest("Invalid credentials");
        }

        var jwtSecurityToken = _authService.CreateJwtToken(user);

        _authService.SetTokensInsideCookie(HttpContext, jwtSecurityToken);

        return Ok(new { UserId = user.Id.ToString(), user.Email });
    }

    [HttpGet("validate-token")]
    [Authorize]
    public IActionResult ValidateToken()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(new { Email = email });
    }

    [HttpPost("logout")]
    public IActionResult InvalidateToken()
    {
        _authService.SetEmptyTokensInsideCookie(HttpContext);

        return Ok();
    }
}