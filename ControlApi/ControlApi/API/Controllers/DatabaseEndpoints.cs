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
        var barchartData = await _db.detections
            .Include(d => d.detectionPOIs)
            .Select(d => new    
            {
                d.detectionId,
                d.latitude,
                d.longitude,
                d.trashType,
                d.timeStamp,

                POIs = d.detectionPOIs.Select(p => new
                {
                    p.POI.name,
                    p.POI.category,
                    p.POI.,
                    p.POI.osmId,
                    p.POI.osmId,
                    p.POI.osmId,
                })
            })
            .ToListAsync();

        return Ok(barchartData);
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