using System.Text.Json.Serialization;

namespace ControlApi.API.DTOs;

public sealed class CalendarFeaturesRequest
{
    [JsonPropertyName("feels_like_temp_celsius")]
    public float FeelsLikeTempCelsius { get; set; }

    [JsonPropertyName("actual_temp_celsius")]
    public float ActualTempCelsius { get; set; }

    [JsonPropertyName("wind_force_bft")]
    public float WindForceBft { get; set; }

    [JsonPropertyName("day_of_week")]
    public int DayOfWeek { get; set; }

    [JsonPropertyName("month")]
    public int Month { get; set; }
}

public sealed class CalendarPredictionBatchResponse
{
    [JsonPropertyName("prediction")]
    public List<string> Prediction { get; set; } = new();
}

public sealed class CalendarPredictionDto
{
    public DateOnly date  { get; set; }
    public string   result { get; set; } = null!;       
}