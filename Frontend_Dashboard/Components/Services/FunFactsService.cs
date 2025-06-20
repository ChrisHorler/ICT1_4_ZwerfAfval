using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services;

public sealed class FunFactsService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public FunFactsService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<FunFactsDto?> GetFactsAsync(DateOnly date)
    {
        var key = $"facts-{date:yyyy-MM-dd}";
        if (_cache.TryGetValue(key, out FunFactsDto cached))
            return cached;

        var all = await _httpClient.GetFromJsonAsync<List<FactRow>>("api/Detections/facts")
                  ?? new();

        var rows = all.Where(r => DateOnly.FromDateTime(r.timeStamp) == date).ToList();
        if (rows.Count == 0) return null;

        var mostCommon = rows.GroupBy(r => r.trashType)
            .OrderByDescending(g => g.Count())
            .First().Key;

        var dto = new FunFactsDto
        {
            Date = date,
            TotalDetections = rows.Count,
            MostCommonTrash = mostCommon,
        };

        _cache.Set(key, dto, TimeSpan.FromMinutes(10));
        return dto;
    }
}