namespace ControlApi.ControlApi.Data.Entities;

public class Detection
{
    public int detectionId { get; set; }
    public int trashTypeId { get; set; }
    public DateTime timeStamp { get; set; }
    
    public float confidence { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    
    public float? feelsLikeTempC { get; set; }
    public float? actualTempC { get; set; }
    public float? windForceBft { get; set; }
    public float? windDirection { get; set; }
    
    public TrashType? trashType { get; set; }
    public ICollection<Prediction> predictions { get; set; } = new List<Prediction>();
    public ICollection<DetectionPOI> detectionPOIs { get; set; } = new List<DetectionPOI>();

}