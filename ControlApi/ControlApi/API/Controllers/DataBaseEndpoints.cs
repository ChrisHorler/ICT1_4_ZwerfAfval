using Microsoft.AspNetCore.Mvc;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.API.Controllers;

[HttpGet("basicTrashInfo")]
public async Task<ActionResult<IEnumerable<Detection>>> GetBasicDetections()
{
    var detections = await _db.detections
        .Select(d => new {
            d.detectionId,
            d.timeStamp,
            d.feelsLikeTempC,
            d.trashType
        })
        .ToListAsync();

    return Ok(detections);
}