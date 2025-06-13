namespace Zwerfafval_WebApp.Components.Models
{
    public class DetectionData
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
        public double? Confidence { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? FeelsLikeTempCelsius { get; set; }
        public double? ActualTempCelsius { get; set; }
        public int? WindForceBft { get; set; }
        public string? WindDirection { get; set; }
    }
}
