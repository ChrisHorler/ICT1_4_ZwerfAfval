using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services;

public sealed class FunFactsService
{
    private readonly HttpClient   _http;
    private readonly IMemoryCache _cache;
    private readonly string       _apiRoot;

    public FunFactsService(HttpClient http,
        IMemoryCache cache,
        IConfiguration cfg)
    {
        _http    = http;
        _cache   = cache;
        _apiRoot = cfg["BackendAPI:BaseUrl"]
                   ?? Environment.GetEnvironmentVariable("BackendAPI:BaseUrl")
                   ?? throw new InvalidOperationException("BackendAPI:BaseUrl is not set");
    }

    public async Task<FunFactsDto?> GetFactsAsync(DateOnly date,
        CancellationToken ct = default)
    {
        var key = $"facts-{date:yyyy-MM-dd}";
        if (_cache.TryGetValue(key, out FunFactsDto cached))
            return cached;
        
        var url   = $"{_apiRoot}api/Detections/facts?date={date:yyyy-MM-dd}";
        var rows = await _http.GetFromJsonAsync<List<FactRow>>(url, ct)
                   ?? new();


        if (rows.Count == 0) return null;
        
        var topTrash = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.trashType))
            .GroupBy(r => r.trashType)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "(unkown))";
            

        var dto = new FunFactsDto
        {
            Date            = date,
            TotalDetections = rows.Count,
            MostCommonTrash = topTrash
        };

        _cache.Set(key, dto, TimeSpan.FromMinutes(10));
        return dto;
    }

    public async Task<(string poiName, int total)> GetTopPoiAsync(DateOnly date, CancellationToken ct = default)
    {
        var url   = $"{_apiRoot}api/Detections/barchart?date={date:yyyy-MM-dd}";
        var list  = await _http.GetFromJsonAsync<List<BarChartDto>>(url, ct) ?? new();

        return list
            .Select(dto => new { dto.name, Count = dto.TrashTypeCounts?.Values.Sum() ?? 0 })
            .OrderByDescending(x => x.Count)
            .Select(x => (x.name, x.Count))
            .FirstOrDefault();
    }
}