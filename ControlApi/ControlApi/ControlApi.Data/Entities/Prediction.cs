namespace ControlApi.ControlApi.Data.Entities;

public class Prediction
{
    public int predictionId { get; set; }
    public int detectionId { get; set; }
    public DateTime predictedFor { get; set; }
    
    public string modelVersion { get; set; } = string.Empty;
    public float predictedFillLevel { get; set; }
    public float confidence { get; set; }
    
    public Detection? detection { get; set; }
}