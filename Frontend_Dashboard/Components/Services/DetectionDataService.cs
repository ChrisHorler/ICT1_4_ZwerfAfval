using System.Net.Http;
using System.Net.Http.Json;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services
{
    public class DetectionDataService : IDetectionDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiUrl;

        public DetectionDataService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiUrl = config["BackendAPI:BaseUrl"] 
                             ?? Environment.GetEnvironmentVariable("BackendAPI:BaseUrl") 
                             ?? throw new InvalidOperationException("'BackendAPI:BaseUrl' not found");
        }

        public async Task<List<DetectionData>> GetDetectionDataAsync()
        {
            try
            {
                // Api endpoint naam hieronder veranderen wanneer deze bekend is 
                var response = await _httpClient.GetFromJsonAsync<List<DetectionData>>($"{this._apiUrl}api/Detections/barchart?date=2025-06-20");
                Console.WriteLine(response);
                return response ?? new List<DetectionData>();
            }
            catch (Exception)
            {
                Console.Write($"[DetectionDataService] Error while fetching data");
                return new List<DetectionData>();
            }
        }
    }
}
