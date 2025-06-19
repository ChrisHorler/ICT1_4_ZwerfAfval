using ControlApi.API.DTOs;
using System.Threading;

namespace ControlApi.API.Services;

public interface IPredictionApiClient
{
    Task<CalendarPredictionResponse> PredictCalendarAsync(
        CalendarFeaturesRequest features,
        CancellationToken ct = default);
    
    Task<Predictions> PredictCalendarBatchAsync(
        List<CalendarFeaturesRequest> features,
        CancellationToken ct = default);
}