using System.Net.Http.Json;
using ControlApi.API.DTOs;

namespace ControlApi.API.Services;

public sealed class PredictionApiClient : IPredictionApiClient
{
    private readonly HttpClient _httpClient;
    public PredictionApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<CalendarPredictionResponse> PredictCalendarAsync(
        CalendarFeaturesRequest features,
        CancellationToken ct = default)
    {
        using var resp = await _httpClient.PostAsJsonAsync("/predict/calendar", features, ct);
        return await resp.Content.ReadFromJsonAsync<CalendarPredictionResponse>(ct)!;
    }

    public async Task<Predictions> PredictCalendarBatchAsync(
        List<CalendarFeaturesRequest> features,
        CancellationToken ct = default)
    {
        using var resp = await _httpClient.PostAsJsonAsync("/predict/calendar/batch", features, ct);
        
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<Predictions>(ct)!;
    }
}