using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.Data;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using System.Linq;
using ControlApi.Data.Entities;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PredictEndpoint : ControllerBase
{
    private readonly ControlApiDbContext   _db;
    private readonly IPredictionApiClient  _python;

    public PredictEndpoint(ControlApiDbContext db, IPredictionApiClient python)
    {
        _db     = db;
        _python = python;
    }

    /// <summary>
    /// GET /api/PredictEndpoint/calendar?date=YYYY-MM-DD
    /// Returns a single "low|medium|high" for the requested day.
    /// </summary>
    [HttpGet("calendar")]
    public async Task<ActionResult<CalendarPredictionDto>> Calendar(
        [FromQuery] DateOnly date,
        CancellationToken   ct)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end   = start.AddDays(1);
        
        var existing = await _db.predictions
            .Where(p => p.predictedFor >= start && p.predictedFor < end)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
            return Ok(new CalendarPredictionDto { date = date, result = existing.confidence });
        
        var rows = await _db.detections
            .Where(d => d.timeStamp >= start && d.timeStamp < end &&
                        d.feelsLikeTempC != null &&
                        d.actualTempC    != null &&
                        d.windForceBft   != null)
            .OrderBy(d => d.timeStamp)
            .ToListAsync(ct);

        if (!rows.Any())
            return NotFound($"No detections for {date:yyyy-MM-dd}");
        
        var features = rows.Select(d => new CalendarFeaturesRequest
        {
            FeelsLikeTempCelsius = d.feelsLikeTempC!.Value,
            ActualTempCelsius    = d.actualTempC!.Value,
            WindForceBft         = d.windForceBft!.Value,
            DayOfWeek            = ((int)d.timeStamp.DayOfWeek + 6) % 7,
            Month                = d.timeStamp.Month
        }).ToList();
        
        var batch = await _python.PredictCalendarBatchAsync(features, ct);
        
        var winner = batch.Prediction
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .First().Key;
        
        _db.predictions.Add(new Prediction
        {
            detectionId  = rows.First().detectionId,
            predictedFor = start,
            modelVersion = "calendar-v1",
            confidence   = winner
        });
        await _db.SaveChangesAsync(ct);

        return Ok(new CalendarPredictionDto
        {
            date  = date,
            result = winner
        });
    }
}
