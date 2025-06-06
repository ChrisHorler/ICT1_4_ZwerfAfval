using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatheringController : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly IJwtService _jwt;

    public GatheringController(ControlApiDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    
    [HttpGet]
    public async Task<ActionResult<AuthResponse>> Register()
    {
        
        return Ok(new GatheringResponse(true, "Ok"));
    }
}