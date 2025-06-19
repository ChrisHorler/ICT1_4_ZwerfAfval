using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.Data;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data.Entities;

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

    // GET /api/PredictEndpoint/calendar?date=YYYY-MM-DD
    [HttpGet("calendar")]
    public async Task<ActionResult<CalendarPredictionDto>> GetCalendarPrediction(
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        
        // Check DB for Cached Predictions
        var dateStart = date.ToDateTime(TimeOnly.MinValue);
        var dateEnd = dateStart.AddDays(1);

        var cached = await _db.predictions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.predictedFor >= dateStart && p.predictedFor <= dateEnd, ct);

        if (cached != null)
        {
            return Ok(new CalendarPredictionDto
            {
                date = date,
                prediction = cached.confidence
            });
        }
        
        // Collect all detections for the day
        var detections = await _db.detections
            .Where(d => d.timeStamp >= dateStart && d.timeStamp < dateEnd &&
                        d.feelsLikeTempC != null &&
                        d.actualTempC != null &&
                        d.windForceBft != null)
            .ToListAsync(ct);

        if (!detections.Any())
            return NotFound($"No valid detections for {date:yyyy-MM-dd}");

        // Map every detection to a FeaturesRequest
        var featureList = detections.Select(d => new CalendarFeaturesRequest
        {
            TimeStamp = DateOnly.FromDateTime(d.timeStamp),
            FeelsLikeTempCelsius = d.feelsLikeTempC!.Value,
            ActualTempCelsius = d.actualTempC!.Value,
            WindForceBft = d.windForceBft!.Value,
            DayOfWeek = ((int)d.timeStamp.DayOfWeek + 6) % 7,
            Month = d.timeStamp.Month
        }).ToList();
        
        var batchResult = await _python.PredictCalendarBatchAsync(featureList, ct);

        if (batchResult != null || batchResult.predictions.Count == 0)
            return Problem("Python API returned no Predictions");

        var finalPrediction = batchResult.predictions
            .GroupBy(p => p)
            .OrderByDescending(g => g.Count())
            .First().Key;
        
        _db.predictions.Add(new Prediction
        {
            detectionId = detections.First().detectionId,
            predictedFor = dateStart,
            modelVersion = "calendar_model.pkl",
            confidence = finalPrediction
        });
        
        await _db.SaveChangesAsync(ct);

        return Ok(new CalendarPredictionDto
        {
            date = date,
            prediction = finalPrediction
        });
    }
}