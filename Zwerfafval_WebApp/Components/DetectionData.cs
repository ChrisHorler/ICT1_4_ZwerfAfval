namespace Zwerfafval_WebApp.Components
{
    public class DetectionData
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public double Confidence { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double? FeelsLikeTempCelsius { get; set; }
        public double? ActualTempCelsius { get; set; }
        public double? WindForceBft { get; set; }
        public string WindDirection { get; set; }
    }

}
