using System.Net.Http.Json;
using Frontend_Dashboard.Components.Models;
using Microsoft.Extensions.Caching.Memory;

public class CalendarService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public CalendarService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<Dictionary<DateOnly,string>> GetPredictionsForMonthAsync(int year, int month)
    {
        var cacheKey = $"{year}-{month:00}";

        if (_cache.TryGetValue(cacheKey, out Dictionary<DateOnly,string>? cached))
            return cached;

        var fresh = await FetchMonthParallelAsync(year, month);
        
        _cache.Set(cacheKey, fresh,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                Size = 1
            });

        // warm-up them neighbours, nice and toasty
        _ = WarmUpAdjacentMonths(year, month);

        return fresh;
    }
    
    private async Task<Dictionary<DateOnly,string>> FetchMonthParallelAsync(int year, int month)
    {
        var days  = DateTime.DaysInMonth(year, month);

        var tasks = Enumerable.Range(1, days).Select(async day =>
        {
            var date = new DateOnly(year, month, day);

            try
            {
                var dto = await _httpClient.GetFromJsonAsync<CalendarPredictionDTO>(
                    $"api/PredictEndpoint/calendar?date={date:yyyy-MM-dd}");

                return (date, dto?.result?.ToLower());
            }
            catch
            {
                return (date, null);
            }
        });

        var results = await Task.WhenAll(tasks);

        return results
            .Where(r => r.Item2 is not null)
            .ToDictionary(r => r.Item1, r => r.Item2!);
    }


    private async Task WarmUpAdjacentMonths(int year, int month)
    {
        var prev = new DateTime(year, month, 1).AddMonths(-1);
        var next = new DateTime(year, month, 1).AddMonths(+1);
        
        if (!_cache.TryGetValue($"{prev:yyyy-MM}", out _))
            _ = FetchMonthParallelAsync(prev.Year, prev.Month);
        
        if (!_cache.TryGetValue($"{next:yyyy-MM}", out _))
            _ = FetchMonthParallelAsync(next.Year, next.Month);
        
        await Task.CompletedTask;
    }

    // Debug for checking Calendar Caching
    public void ClearCache()
    {
        if (_cache is MemoryCache mem)
            mem.Compact(1.0);
    }
}