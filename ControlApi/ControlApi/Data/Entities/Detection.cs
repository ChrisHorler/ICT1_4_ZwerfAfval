namespace ControlApi.Data.Entities;

public class RawDetection
{
    public DateTime timeStamp { get; set; }
    
    public float confidence { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    
    public float? feelsLikeTempC { get; set; }
    public float? actualTempC { get; set; }
    public float? windForceBft { get; set; }
    public float? windDirection { get; set; }
    
    public string trashType { get; set; } = string.Empty;
}

public class Detection: RawDetection
{

    public ICollection<Prediction> predictions { get; set; } = new List<Prediction>();
    public ICollection<DetectionPOI> detectionPOIs { get; set; } = new List<DetectionPOI>();

}