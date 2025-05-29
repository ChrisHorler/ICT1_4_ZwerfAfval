namespace ControlApi.Data.Entities;

public class DetectionPOI
{
    public int detectionId { get; set; }
    public int POIID { get; set; }
    public float distanceM { get; set; }
    public bool isNearest { get; set; }
    
    public Detection? detection { get; set; }
    public POI? POI { get; set; }
}