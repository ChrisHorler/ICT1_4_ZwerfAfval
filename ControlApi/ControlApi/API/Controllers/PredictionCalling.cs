async Task<string> GetPredictionFromFastAPI(float feels, float actual, float wind, int day, int month)
{
    var client = new HttpClient();
    var request = new
    {
        feels_like_temp_celsius = feels,
        actual_temp_celsius = actual,
        wind_force_bft = wind,
        day_of_week = day,
        month = month
    };

    var response = await client.PostAsJsonAsync("http://localhost:8000/predict/calendar", request);
    if (!response.IsSuccessStatusCode)
    {
        // handle error, maybe fallback to "unknown"
        return "unknown";
    }

    var json = await response.Content.ReadFromJsonAsync<PredictionResponse>();
    return json?.prediction ?? "unknown";
}

public class PredictionResponse
{
    public string prediction { get; set; } = "unknown";
}