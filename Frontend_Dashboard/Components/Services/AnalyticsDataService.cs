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

        public async Task<List<BarChartDto>> GetBarChartDataAsync()
        {
            try
            {
                // Api endpoint naam hieronder veranderen wanneer deze bekend is 
                var response = await _httpClient.GetFromJsonAsync<List<BarChartDto>>($"{this._apiUrl}api/Detections/barchart?date=2025-06-20");
                return response ?? new List<BarChartDto>();
            }
            catch (Exception)
            {
                this._logger.LogError($"[DetectionDataService] Error while fetching barchart data");
                return new List<BarChartDto>();
            }
        }

        public async Task<List<DetectionData>> GetLineGraphDataAsync()
        {
            try
            {
                // Pas de API-endpoint aan als nodig
                var response = await _httpClient.GetFromJsonAsync<List<DetectionData>>($"{this._apiUrl}api/Detections/linegraph?date=2025-06-20");
                return response ?? new List<DetectionData>();
            }
            catch (Exception)
            {
                this._logger.LogError("[AnalyticsDataService] Error while fetching line graph data");
                return new List<DetectionData>();
            }
        }
    }
}
