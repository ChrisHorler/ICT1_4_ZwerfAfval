using System.Net.Http.Json;
using ControlApi.API.DTOs;

namespace ControlApi.API.Services;

public interface IPredictionApiClient
{
    Task<CalendarPredictionResponse> PredictCalendarAsync(
        CalendarFeaturesRequest features,
        CancellationToken        ct = default);
}

public sealed class PredictionApiClient : IPredictionApiClient
{
    private readonly HttpClient _http;

    public PredictionApiClient(HttpClient http) => _http = http;

    public async Task<CalendarPredictionResponse> PredictCalendarAsync(
        CalendarFeaturesRequest features,
        CancellationToken       ct = default)
    {
        using var response = await _http.PostAsJsonAsync("/predict/calendar", features, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CalendarPredictionResponse>(cancellationToken: ct))!;
    }
}