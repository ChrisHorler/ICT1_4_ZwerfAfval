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
public class GatheringController : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly IJwtService _jwt;
    private readonly SensoringConnector _sensoringConnector;
    private readonly ILogger<GatheringController> _logger;

    public GatheringController(ControlApiDbContext db, IJwtService jwt,
        IHttpClientFactory httpClientFactory, ILogger<SensoringConnector> SCLogger,
        ILogger<GatheringController> logger, IConfiguration config
    )
    {
        _db = db;
        _jwt = jwt;
        _logger = logger;
        _sensoringConnector = new SensoringConnector(httpClientFactory, SCLogger, config);
    }

    
    [HttpGet]
    public async Task<ActionResult<AuthResponse>> Register()
    {
        this._logger.LogInformation("Registering Gathering");
        return Ok(new GatheringResponse(true, "Ok"));
    }
}