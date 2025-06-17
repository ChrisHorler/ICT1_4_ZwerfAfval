using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.Data;
using ControlApi.API.DTOs;
using ControlApi.API.Services;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PredictEndpoint : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly IPredictionApiClient _python;

    public PredictEndpoint(ControlApiDbContext db, IPredictionApiClient python)
    {
        _db = db;
        _python = python;
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<CalendarPredictionResponse>> GetCalendarPrediction(
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var detection = await _db.detections
            .Where(d => d.timeStamp.Date == date.ToDateTime(TimeOnly.MinValue))
            .OrderBy(d => d.timeStamp)
            .FirstOrDefaultAsync(ct);

        if (detection is null)
            return NotFound($"No Detection for {date:yyyy-MM-dd}");

        var features = new CalendarFeaturesRequest
        {
            FeelsLikeTempCelsius = detection.feelsLikeTempC,
            ActualTempCelsius = detection.actualTempC,
            WindForceBft = detection.windForceBft,
            DayOfWeek = ((int)date.DayOfWeek + 6) % 7,
            Month = date.Month
        };
        
        var pyResponse = await _python.PredictCalendarAsync(features, ct);

        return Ok(new CalendarPredictionDto
        {
            date = date,
            prediction = pyResponse.Prediction,
        });
    }
}