using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlApi.Data.Entities;

public class RawDetection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? detectionId { get; set; }
    public DateTime timeStamp { get; set; }
    
    public float confidence { get; set; }
    public float? latitude { get; set; }
    public float? longitude { get; set; }
    
    public float? feelsLikeTempC { get; set; }
    public float? actualTempC { get; set; }
    public float? windForceBft { get; set; }
    public float? windDirection { get; set; }
    
    public string trashType { get; set; } = string.Empty;
}

public class TempDetection: RawDetection
{
    public ICollection<Prediction>? predictions { get; set; } = new List<Prediction>();
    public ICollection<DetectionPOI>? detectionPOIs { get; set; } = new List<DetectionPOI>();
    
    public Detection ConvertToDetection()
    {
        return new Detection
        {
            detectionId = null,
            timeStamp = this.timeStamp,
            confidence = this.confidence,
            latitude = this.latitude,
            longitude = this.longitude,
            feelsLikeTempC = this.feelsLikeTempC,
            actualTempC = this.actualTempC,
            windForceBft = this.windForceBft,
            windDirection = this.windDirection,
            trashType = this.trashType,
            predictions  = this.predictions ?? new List<Prediction>(),
            detectionPOIs = this.detectionPOIs ?? new List<DetectionPOI>()
        };
    }
}

public class Detection: RawDetection
{
    public ICollection<Prediction> predictions { get; set; } = new List<Prediction>();
    public ICollection<DetectionPOI> detectionPOIs { get; set; } = new List<DetectionPOI>();

}