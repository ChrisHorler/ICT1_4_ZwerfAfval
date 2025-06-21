using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.API.DTOs;
using ControlApi.Data;
using ControlApi.Data.Entities;
using System.Linq;

namespace ControlApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetectionsController : ControllerBase
{
    private readonly ControlApiDbContext _db;

    public DetectionsController(ControlApiDbContext db)
    {
        _db = db;
    }

    [HttpGet("facts")]
    public async Task<ActionResult<List<DetectionDto>>> GetFactsData(
       [FromQuery] DateOnly date,
       CancellationToken ct)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        var factData = await _db.detections
            .Where(d => d.timeStamp.Date == dateTime.Date)
            .Select(d => new DetectionDto(
                d.detectionId,
                d.timeStamp,
                d.trashType,
                d.latitude,
                d.longitude
            ))
            .ToListAsync(ct);

        if (!factData.Any())
            return NotFound($"No detections found for {date:yyyy-MM-dd}");

        return Ok(factData);
    }



    [HttpGet("barchart")]
    public async Task<ActionResult<IEnumerable<BarchartDto>>> GetBarchartData(
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        var windowPOIs = await _db.detectionPOIs
            .Include(dp => dp.POI)
            .Include(dp => dp.detection)
            .Where(dp =>
                dp.timeStamp.Date == dateTime.Date &&
                dp.detection.timeStamp.Date == dateTime.Date
            )
            .Select(dp => new
            {
                PoiId = dp.POIID,
                PoiName = dp.POI!.name,
                PoiCat = dp.POI!.category,
                TrashType = dp.detection!.trashType
            })
            .ToListAsync(ct);

        if (!windowPOIs.Any())
            return NotFound($"No POI data found for {date:yyyy-MM-dd}");

        var result = windowPOIs
            .GroupBy(x => new { x.PoiId, x.PoiName, x.PoiCat })
            .Select(g => new BarchartDto
            {
                Name = g.Key.PoiName,
                Category = g.Key.PoiCat,
                TrashTypeCounts = g
                    .GroupBy(x => x.TrashType)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Count()
                    )
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("linegraph")]
    public async Task<ActionResult<List<LineGraphDto>>> GetLineGraphData(
       [FromQuery] DateOnly date,
       CancellationToken ct)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        var linegraph = await _db.detections
            .Where(d => d.timeStamp.Date == dateTime.Date)
            .Select(d => new LineGraphDto(
                d.detectionId,
                d.timeStamp
            ))
            .ToListAsync(ct);

        if (!linegraph.Any())
            return NotFound($"No line graph data found for {date:yyyy-MM-dd}");

        return Ok(linegraph);
    }
}