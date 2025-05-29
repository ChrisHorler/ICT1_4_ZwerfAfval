using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlApi.Data.Entities;
using Microsoft.IdentityModel.Tokens;

namespace ControlApi.API.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

public class JwtService : IJwtService
{
    private readonly string _key;

    public JwtService(IConfiguration config)
    {
        _key = config["Jwt:Key"]
            ?? Environment.GetEnvironmentVariable("JWT_KEY")
            ?? throw new InvalidOperationException("JWT_KEY not found");
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.email),
        };
        
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(12),
            claims: claims,
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}