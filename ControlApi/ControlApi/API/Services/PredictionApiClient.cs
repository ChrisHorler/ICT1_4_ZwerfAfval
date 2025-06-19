using System.Net.Http.Json;
using ControlApi.API.DTOs;

namespace ControlApi.API.Services;

public interface IPredictionApiClient
{
    Task<CalendarPredictionBatchResponse> PredictCalendarBatchAsync(
        List<CalendarFeaturesRequest> features,
        CancellationToken ct = default);
}

public sealed class PredictionApiClient : IPredictionApiClient
{
    private readonly HttpClient _http;
    public PredictionApiClient(HttpClient http) => _http = http;

    public async Task<CalendarPredictionBatchResponse> PredictCalendarBatchAsync(
        List<CalendarFeaturesRequest> features, CancellationToken ct = default)
    {
        var rsp = await _http.PostAsJsonAsync("/predict/calendar/batch", features, ct);
        rsp.EnsureSuccessStatusCode();
        return (await rsp.Content.ReadFromJsonAsync<CalendarPredictionBatchResponse>(cancellationToken: ct))!;
    }
}