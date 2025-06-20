using System.Data;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using ControlApi.SensoringConnection.Models;
using ControlApi.SensoringConnection.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DailyBackgroundService> _logger;
    private readonly SensoringConnector _sensoringConnector;
    private readonly bool _isTest;

    public DebugController(ControlApiDbContext db, IJwtService jwt, IHttpClientFactory httpClientFactory, 
        ILogger<SensoringConnector> modelLogger,  IConfiguration config,
        IServiceScopeFactory scopeFactory)
    {
        _httpClientFactory = httpClientFactory;
        _sensoringConnector= new SensoringConnector(_httpClientFactory, modelLogger, config, scopeFactory);
        _isTest = config.GetValue<bool>("Testing");
        _db = db;
        _jwt = jwt;
    }

    
    [HttpPost("pullAsync")]
    public async Task<ActionResult<PullAsyncResponseDto>> Pull(PullAsyncRequestDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            await _sensoringConnector.PullAsync(cancellationToken, dto);
        }
        catch (DataException excpt)
        {
            return BadRequest(new PullAsyncResponseDto(false));
        }

        return Ok(new PullAsyncResponseDto(true));
    }
}