namespace ControlApi.Data.Entities;

public class DetectionPOI
{
    public int detectionId { get; set; }
    public int POIID { get; set; }
    public float detectionRadiusM { get; set; }
    
    public Detection? detection { get; set; }
    public POI? POI { get; set; }
}