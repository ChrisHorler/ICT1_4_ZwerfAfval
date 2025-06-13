using System.Net.Http;
using System.Net.Http.Json;
using Zwerfafval_WebApp.Components.Models;

namespace Zwerfafval_WebApp.Components.Services
{
    public class WasteCalendarService : IWasteCalendarService
    {
        private readonly HttpClient _httpClient;

        public WasteCalendarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WasteCalendarData>> GetDetectionDataAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<WasteCalendarData>>("control-api/prediction"); //change if needed
            return response ?? new List<WasteCalendarData>();
        }
    }
}