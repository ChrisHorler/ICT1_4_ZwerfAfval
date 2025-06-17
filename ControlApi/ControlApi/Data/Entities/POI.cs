using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlApi.Data.Entities;

public class POI
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int POIID { get; set; }
    public string category { get; set; } // this is a translationf rom the `tags.highway` and `tags.amenity`
    public long osmId { get; set; } // openstreetmap ID
    public string? name { get; set; } // only trashbins don't have names
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string source { get; set; } = "overpass";
    public DateTime retrievedAt { get; set; }
    public ICollection<DetectionPOI> detectionPOIs { get; set; } = new List<DetectionPOI>();
}