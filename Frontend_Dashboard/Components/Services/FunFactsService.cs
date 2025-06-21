using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services;

public sealed class FunFactsService
{
    private readonly HttpClient     _http;
    private readonly IMemoryCache   _cache;

    public FunFactsService(HttpClient http, IMemoryCache cache)
    {
        _http  = http;
        _cache = cache;
    }

    public async Task<FunFactsDto?> GetFactsAsync(DateOnly date, CancellationToken ct = default)
    {
        var key = $"facts-{date:yyyy-MM-dd}";
        if (_cache.TryGetValue(key, out FunFactsDto dto))
            return dto;
        
        var url = $"api/Detections/facts?date={date:yyyy-MM-dd}";

        try
        {
            var rows = await _http.GetFromJsonAsync<List<DetectionData>>(url, ct)
                       ?? new();

            
            if (rows.Count == 0) return null;

            
            var mostCommon = rows
                .GroupBy(r => r.Type)
                .OrderByDescending(g => g.Count())
                .First().Key;

            dto = new FunFactsDto
            {
                Date             = date,
                TotalDetections  = rows.Count,
                MostCommonTrash  = mostCommon
            };

            _cache.Set(key, dto, TimeSpan.FromMinutes(10));
            return dto;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
