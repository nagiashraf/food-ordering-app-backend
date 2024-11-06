using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodOrderingApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FoodOrderingApp.Auth;

public class AuthService
{
    private readonly JWT _jwt;
    private readonly IWebHostEnvironment _env;
    public AuthService(IOptions<JWT> jwt, IWebHostEnvironment env)
    {
        _jwt = jwt.Value;
        _env = env;
    }

    public JwtSecurityToken CreateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    public void SetTokensInsideCookie(HttpContext httpContext, JwtSecurityToken jwtSecurityToken)
    {
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        httpContext.Response.Cookies.Append("auth_token", token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = _env.IsProduction(),
                MaxAge = TimeSpan.FromDays(_jwt.DurationInDays)
            });
    }

    public void SetEmptyTokensInsideCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Append("auth_token", string.Empty,
            new CookieOptions
            {
                MaxAge = TimeSpan.Zero
            });
    }
}