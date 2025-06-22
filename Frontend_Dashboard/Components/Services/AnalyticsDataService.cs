using System.Net.Http;
using System.Net.Http.Json;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services
{
    public class AnalyticsDataService : IAnalyticsDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiUrl;
        private readonly ILogger<AnalyticsDataService> _logger;

        public AnalyticsDataService(HttpClient httpClient, IConfiguration config,
            ILogger<AnalyticsDataService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
            _apiUrl = config["BackendAPI:BaseUrl"] 
                             ?? Environment.GetEnvironmentVariable("BackendAPI:BaseUrl") 
                             ?? throw new InvalidOperationException("'BackendAPI:BaseUrl' not found");
        }

        public async Task<List<BarChartDto>> GetBarChartDataAsync(DateOnly date)
        {
            try
            {
                var url = $"{_apiUrl}api/Detections/barchart?date={date:yyyy-MM-dd}";
                var response = await _httpClient.GetFromJsonAsync<List<BarChartDto>>(url);
                return response ?? new List<BarChartDto>();
            }
            catch (Exception)
            {
                this._logger.LogError($"[DetectionDataService] Error while fetching barchart data");
                return new List<BarChartDto>();
            }
        }

        public async Task<List<LineGraphDto>> GetLineGraphDataAsync(DateOnly from,
            DateOnly to)
        {
            var all = new List<LineGraphDto>();

            // Fire the whole months worth of requests
            var tasks = Enumerable.Range(0, to.DayNumber - from.DayNumber + 1)
                .Select(i => FetchDayAsync(from.AddDays(i)))
                .ToList();

            var results = await Task.WhenAll(tasks);

            foreach (var list in results)
                all.AddRange(list);

            return all;
        }
        
        private async Task<List<LineGraphDto>> FetchDayAsync(DateOnly day)
        {
            try
            {
                var url = $"{_apiUrl}api/Detections/linegraph?date={day:yyyy-MM-dd}";
                return await _httpClient.GetFromJsonAsync<List<LineGraphDto>>(url) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "[Analytics] failed line-graph call for {Date}", day);
                return new();
            }
        }
    }
}
