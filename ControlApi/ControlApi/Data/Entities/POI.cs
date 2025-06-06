using System.ComponentModel.DataAnnotations.Schema;

namespace ControlApi.Data.Entities;

public class POI
{
    public int POIID { get; set; }
    public int categoryId { get; set; }
    public int osmId { get; set; }
    public string elementType { get; set; } = string.Empty;
    public string? name { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    public string source { get; set; } = "overpass";
    public DateTime retrievedAt { get; set; }
    
    [NotMapped]
    public Dictionary<string, string>? tags { get; set; }
    
    public POICategory? category { get; set; }
    public ICollection<DetectionPOI> detectionPOIs { get; set; } = new List<DetectionPOI>();
}