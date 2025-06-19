using System.Net.Http;
using System.Net.Http.Json;
using Frontend_Dashboard.Components.Models;

namespace Frontend_Dashboard.Components.Services
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
            catch (Exception)
            {
                Console.Write($"[DetectionDataService] Error while fetching data");
                return new List<DetectionData>();
            }
        }
    }
}
