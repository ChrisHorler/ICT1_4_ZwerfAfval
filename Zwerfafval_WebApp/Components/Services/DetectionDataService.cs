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
            try
            {
                // Api endpoint naam hieronder veranderen wanneer deze bekend is 
                var response = await _httpClient.GetFromJsonAsync<List<DetectionData>>("control-api/collection");
                return response ?? new List<DetectionData>();
            }
            catch (Exception ex)
            {
                Console.Write($"[DetectionDataService] Error while fetching data");
                return new List<DetectionData>();
            }
        }
    }
}
