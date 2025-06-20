using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.API.DTOs;
using ControlApi.Data;
using ControlApi.Data.Entities;

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

    // Basic endpoint to get raw detection data with filtering capabilities ty Martijn


    [HttpGet("facts")]
    public async Task<ActionResult<IEnumerable<object>>> GetFactsRawData()
    {
        var factData = await _db.detections
            .Select(d => new
            {
                d.detectionId,
                d.latitude,
                d.longitude,
                d.trashType,
                d.timeStamp
            })
            .ToListAsync();

        return Ok(factData);
    }


    [HttpGet("barchart")]
    public async Task<ActionResult<IEnumerable<object>>> GetBarchartRawData()
    {
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endTime = DateTime.UtcNow.AddYears(1);

        var windowPOIs = await _db.detectionPOIs
            .Include(dp => dp.POI)
            .Include(dp => dp.detection)
            .Where(dp =>
                dp.timeStamp >= startTime &&
                dp.timeStamp <= endTime &&
                dp.detection.timeStamp >= startTime &&
                dp.detection.timeStamp <= endTime
            )
            .Select(dp => new
            {
                PoiId = dp.POIID,
                PoiName = dp.POI!.name,
                PoiCat = dp.POI!.category,
                TrashType = dp.detection!.trashType
            })
            .ToListAsync();

        // 2) Group in memory by POI, then build your dictionary of trash‑type counts
        var result = windowPOIs
            .GroupBy(x => new { x.PoiId, x.PoiName, x.PoiCat })
            .Select(g => new
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
  
    [HttpGet("linegraphData")]
    public async Task<ActionResult<IEnumerable<object>>> GetLineGraphRawData()
    {
        var linegraphData = await _db.detections
            .Select(d => new
            {
                d.detectionId,
                d.timeStamp
            })
            .ToListAsync();

        return Ok(linegraphData);
    }
}