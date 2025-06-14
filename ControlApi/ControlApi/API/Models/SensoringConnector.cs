using System.Text;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ControlApi.SensoringConnection.Models;

public class SensoringConnector
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SensoringConnector> _logger;
    private readonly IJwtService _jwt;
    private readonly string _apiUrl;
    private readonly IServiceScopeFactory _scopeFactory;

    public SensoringConnector(
        IHttpClientFactory httpClientFactory,
        ILogger<SensoringConnector> logger,
        IConfiguration config,
        IServiceScopeFactory scopeFactory
    )
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiUrl = config["SENSORING_API"]
                  ?? Environment.GetEnvironmentVariable("SENSORING_API")
                  ?? throw new InvalidOperationException("SENSORING_API not found");

    }

    public async Task PullAsync(CancellationToken cancellationToken)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ControlApiDbContext>();
            if (dbContext == null)
            {
                this._logger.LogInformation("Tried to pull data from Sensoring API without DbContext.");
                return;
            }

            var latestItem = dbContext.detections
                .OrderByDescending(e => e.timeStamp)
                .FirstOrDefault();
            if (latestItem == null)
            {
                latestItem = new Detection();
                latestItem.timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            this._logger.LogInformation("Updating our Trash Collection with the Sensoring API.\nLastUpdate: {item}",
                latestItem.timeStamp);
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(this._apiUrl, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    ApiResponse parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(data);
                    _logger.LogInformation("Received data from external API: {parsedResponse}", data);
                    List<TempDetection> trashDetections = SensoringConvertor.ConvertFullModel(parsedResponse);
                    // now populate it with locationdata, 50m radius

                    // we doen een prediction based op front-end request.
                    // een prediction haalt alle data op uit de db, en stuurt het naar de AI
                    // caching the output in the db. (want we willen maar 1 prediction per X min.
                    
                    


                    // dbContext.detections.AddRange(trashDetections);
                    // dbContext.SaveChanges();
                }
                catch (JsonException exception)
                {
                    _logger.LogError("Received data from external API, it is NOT Deserializable: {Data}", data);
                }
            }
            else
            {
                _logger.LogWarning("Failed to fetch data from external API. Status code: {StatusCode}",
                    response.StatusCode);
            }
        }
    }

    /// <summary>
    /// Queries the Overpass API for restaurants, bus stops, and train stations near the given point.
    /// </summary>
    static async Task<List<JObject>> QueryNearbyElementsAsync(double lat, double lon, int radius)
    {
        string overpassQuery = $@"
[out:json];
(
  node[""amenity""=""restaurant""](around:{radius},{lat},{lon});
  node[""highway""=""bus_stop""](around:{radius},{lat},{lon});
  node[""railway""=""station""](around:{radius},{lat},{lon});
);
out body;
";


        using (var client = new HttpClient())
        {
            var content = new StringContent($"data={Uri.EscapeDataString(overpassQuery)}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            // Send the POST
            var response = await client.PostAsync("https://overpass-api.de/api/interpreter", content);
            response.EnsureSuccessStatusCode();

            // Parse JSON
            var json = await response.Content.ReadAsStringAsync();
            var root = JObject.Parse(json);
            var elements = (JArray)root["elements"];

            // Convert to list of JObject for easier handling
            var list = new List<JObject>();
            foreach (var el in elements)
                list.Add((JObject)el);

            return list;
        }
    }
}