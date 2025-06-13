using System.Net.Http;
using System.Net.Http.Json;
using Zwerfafval_WebApp.Components.Models;

namespace Zwerfafval_WebApp.Components.Services
{
    public class DetectionDataService : IDetectionDataService
    {
        private readonly HttpClient _httpClient;

        public DetectionDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DetectionData>> GetDetectionDataAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<DetectionData>>("control-api/collection");
            return response ?? new List<DetectionData>();
        }
    }
}
