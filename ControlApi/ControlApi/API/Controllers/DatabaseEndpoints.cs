using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlApi.API.DTOs;
using ControlApi.Data;
using ControlApi.Data.Entities;

namespace ControlApi.API.Controllers;


// For now this thing that four main endpoints that serve the db information to the front end, wasn't sure if this makes sense.
[ApiController]
[Route("api/[controller]")]
public class DatabaseEndpoints : ControllerBase
{
    private readonly ControlApiDbContext _db;
    private readonly ILogger<DatabaseEndpoints> _logger; // geen idee of we dit nodig hebben 

    public DatabaseEndpoints(ControlApiDbContext db, ILogger<DatabaseEndpoints> logger)
    {
        _db = db;
        _logger = logger;
    }


// date filter toevoegen 


    // FUN FACT PART WOO // change to month 
    [HttpGet("facts")]
    public async Task<ActionResult<FunFacts>> GetFunFacts([FromQuery] DateTime selectedDay)
    {

        var detections = await _db.detections
            .Where(d => d.timeStamp.Year == year && d.timeStamp.Month == month)
            .ToListAsync();

        //queries database foor POI and decetionPOI, checks if it falls within that week
        var detections = await _db.detections
            .Include(d => d.detectionPOIs)
            .ThenInclude(dp => dp.POI)
            .Where(d => d.timeStamp >= weekStart && d.timeStamp < weekEnd)
            .ToListAsync();

        // if not then this 
        if (!detections.Any()) return NotFound("No data available for selected week.");

        // Most trashed up location by filtering location with atleast one POI, Groups them, orders by frequency, then takes the spot.  
        var mostCommonLocation = detections
            .Where(d => d.detectionPOIs.Any())
            .GroupBy(d => d.detectionPOIs.First().POI?.name ?? "Unknown")
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "Unknown";

        // Total trash count of a week 
        var totalTrash = detections.Count;

        // Most common trash type, groups detections by type, orders by frequency, returns the most common trashtype. 
        var mostCommonTrash = detections
            .GroupBy(d => d.trashType)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "Unknown";

        return Ok(new FunFacts(
            locationName: mostCommonLocation,
            totalTrashCount: totalTrash,
            mostCommonTrashType: mostCommonTrash
        ));

        // Would look like this then: 
        // {
        // "locationName": "Central Station Breda",
        // "totalTrashCount": 69420,
        // "mostCommonTrashType": "arghhh me cigarette bootie"
        // }
    }

    // 📊 BARCHARTYEEE
    [HttpGet("barchart")]
    public async Task<ActionResult<List<BarchartInfo>>> GetBarchart([FromQuery] DateTime selectedDay)
    {
        
        var detections = await _db.detections
            .Where(d => d.timeStamp.Year == year && d.timeStamp.Month == month)
            .ToListAsync();

        //The same fetching thing related to the POI
        var detections = await _db.detections
            .Include(d => d.detectionPOIs)
            .ThenInclude(dp => dp.POI)
            .Where(d => d.timeStamp >= weekStart && d.timeStamp < weekEnd)
            .ToListAsync();

        //Groups for each unique POI name, in the dictionary: creates subgroup by trahstype, converts it into {"plastic": 69}
        var result = detections
            .Where(d => d.detectionPOIs.Any())
            .GroupBy(d => d.detectionPOIs.First().POI?.name ?? "Unknown")
            .Select(g => new BarchartInfo(
                poiName: g.Key,
                trashTypeCounts: g.GroupBy(x => x.trashType)
                                   .ToDictionary(tg => tg.Key, tg => tg.Count())
            )).ToList();

        return Ok(result);
    }

    // Returns something like this: 
    // [
    //     {
    //         "poiName": "Breda centraal Park",
    //         "trashTypeCounts": {
    //         "plastic bottles": 23,
    //         "cigarette butts": 45,
    //         "food wrappers": 12
    //         }
    //     },
    //     {
    //         "poiName": "Mcdonalds Breda",
    //         "trashTypeCounts": {
    //         "paper cups": 18,
    //         "newspapers": 9
    //         }
    //     }
    // ]

    // 📈 LINEGRAPHHHHH
    [HttpGet("linegraph")]
    public async Task<ActionResult<List<LineGraphInfo>>> GetLineGraph([FromQuery] int year, [FromQuery] int month)
    {
        // Slightly different from the other endpoints, it looks at the year and month 
        var detections = await _db.detections
            .Where(d => d.timeStamp.Year == year && d.timeStamp.Month == month)
            .ToListAsync();

        var result = detections
            .GroupBy(d => d.timeStamp.Date)
            .Select(g => new LineGraphInfo(
                date: g.Key.ToString("yyyy-MM-dd"),
                totalTrashCount: g.Count()
            )).ToList();

        return Ok(result);
    }
    
        // Returns something like this: 
    // [
    // {"date": "2023-06-01", "totalTrashCount": 42},
    // {"date": "2023-06-02", "totalTrashCount": 35},
    // {"date": "2023-06-03", "totalTrashCount": 28},
    // ...
    // ]

}


// [HttpGet("basicTrashInfo")]
// public async Task<ActionResult<IEnumerable<Detection>>> GetBasicDetections()
// {
//     var detections = await _db.detections
//         .Select(d => new {
//             d.detectionId,
//             d.timeStamp,
//             d.feelsLikeTempC,
//             d.trashType
//         })
//         .ToListAsync();

//     return Ok(detections);
// }