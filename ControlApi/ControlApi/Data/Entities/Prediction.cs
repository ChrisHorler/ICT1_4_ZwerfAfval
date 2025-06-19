namespace ControlApi.Data.Entities;

public class Prediction
{
    public int predictionId { get; set; }
    public int detectionId { get; set; }
    public DateTime predictedFor { get; set; }
    
    public string modelVersion { get; set; } = string.Empty;

    public string confidence { get; set; } = string.Empty;
    
    public Detection? detection { get; set; }
}