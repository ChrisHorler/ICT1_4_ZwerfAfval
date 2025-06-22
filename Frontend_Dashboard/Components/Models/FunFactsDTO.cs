namespace Frontend_Dashboard.Components.Models;

public record FactRow
(
    int       detectionId,
    decimal   latitude,
    decimal   longitude,
    string?   trashType,
    DateTime  timeStamp
);

public record FunFactsDto
{
    public DateOnly Date { get; init; }
    public int TotalDetections { get; init; }
    public string? MostCommonTrash { get; set; }
    
    public string? TopPOIName { get; set; }
    public int TopPoiDetections { get; set; }
    
}