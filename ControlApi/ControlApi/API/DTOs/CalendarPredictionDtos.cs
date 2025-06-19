using System.Text.Json.Serialization;

namespace ControlApi.API.DTOs;

public sealed class CalendarFeaturesRequest{
    
    [JsonPropertyName("timestamp")]
    public DateOnly TimeStamp { get; set; }
    
    [JsonPropertyName("feels_like_temp_celsius")]
    public float? FeelsLikeTempCelsius { get; set; }
    
    [JsonPropertyName("actual_temp_celsius")]
    public float? ActualTempCelsius { get; set; }
    
    [JsonPropertyName("wind_force_bft")]
    public float? WindForceBft { get; set; }
    
    [JsonPropertyName("day_of_week")]
    public int DayOfWeek { get; set; }
    
    [JsonPropertyName("month")]
    public int Month { get; set; }
}

public sealed class CalendarPredictionResponse
{
    [JsonPropertyName("prediction")]
    public string Prediction { get; set; }
}

public sealed class CalendarPredictionDto
{
    public DateOnly date {get; set;}
    public string prediction { get; set; } = string.Empty;
}

public sealed class Predictions
{
    [JsonPropertyName("predictions")]
    public List<string> predictions { get; set; } = new();
}