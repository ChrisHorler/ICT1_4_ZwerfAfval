// dit moet gecomment blijven, anders is het niet runnable

using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly IJwtService _jwt;

    public AuthController(ControlApiDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (await _db.users.AnyAsync(u => u.email == dto.email))
            return Conflict("Invalid Email or Password");

        var user = new User
        {
            email = dto.email,
            passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password),
            createdAt = DateTime.UtcNow,
        };
        
        _db.users.Add(user);
        await _db.SaveChangesAsync();
        
        var token = _jwt.GenerateToken(user);
        return Ok(new AuthResponse(token, user.userId, user.email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await _db.users.SingleOrDefaultAsync(u => u.email == dto.email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.password, user.passwordHash))
            return Conflict ("Invalid Email or Password");
        
        var token = _jwt.GenerateToken(user);
        return Ok(new AuthResponse(token, user.userId, user.email));
    }
}