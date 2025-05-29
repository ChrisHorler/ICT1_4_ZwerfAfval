namespace ControlApi.ControlApi.Data.Entities;

public class POICategory
{
    public int categoryId { get; set; }
    public string name { get; set; } = string.Empty;
    public ICollection<POI> POIS { get; set; } = new List<POI>();
}