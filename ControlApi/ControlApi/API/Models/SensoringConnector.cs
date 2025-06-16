using System.Text;
using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;


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
                    _logger.LogInformation("Parsed data to correct format: {trashDetections}", trashDetections);
                    foreach (var trashDetection in trashDetections)
                    {
                        
                        var responseObj = await this.QueryNearbyElementsAsync(trashDetection.latitude, trashDetection.longitude, 50, dbContext);
                        _logger.LogInformation("Response: {responseObj}", responseObj);
                        var det = trashDetection.ConvertToDetection();
                        
                    }
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
    
    public async Task<POI> GetOrCreatePoiAsync(ControlApiDbContext db, POI newPoi)
    {
        var poi = await db.POIs
            .FirstOrDefaultAsync(p =>
                p.osmId == newPoi.osmId);
        if (poi != null)
        {
            return poi;
        }
        poi = newPoi;
        db.POIs.Add(poi);
        await db.SaveChangesAsync();
        return poi;
    }


    /// <summary>
    /// Queries the Overpass API for restaurants, bus stops, and train stations near the given point.
    /// </summary>
    async Task<List<POI>> QueryNearbyElementsAsync(double lat, double lon, int radius, ControlApiDbContext db)
    {
        string overpassQuery = $@"
[out:json][timeout:25];
// verzamel nodes, ways & relations in één set
(
  node[amenity=restaurant](around:{radius},{lon},{lat});
  way[amenity=restaurant](around:{radius},{lon},{lat});
  relation[amenity=restaurant](around:{radius},{lon},{lat});

  node[highway=bus_stop](around:{radius},{lon},{lat});
  way[highway=bus_stop](around:{radius},{lon},{lat});
  relation[highway=bus_stop](around:{radius},{lon},{lat});

  node[railway=station](around:{radius},{lon},{lat});
  way[railway=station](around:{radius},{lon},{lat});
  relation[railway=station](around:{radius},{lon},{lat});

  node[amenity=cafe](around:{radius},{lon},{lat});
  way[amenity=cafe](around:{radius},{lon},{lat});
  relation[amenity=cafe](around:{radius},{lon},{lat});

  node[amenity=nightclub](around:{radius},{lon},{lat});
  way[amenity=nightclub](around:{radius},{lon},{lat});
  relation[amenity=nightclub](around:{radius},{lon},{lat});

  node[amenity=waste_basket](around:{radius},{lon},{lat});
  way[amenity=waste_basket](around:{radius},{lon},{lat});
  relation[amenity=waste_basket](around:{radius},{lon},{lat});

  node[amenity=fast_food](around:{radius},{lon},{lat});
  way[amenity=fast_food](around:{radius},{lon},{lat});
  relation[amenity=fast_food](around:{radius},{lon},{lat});
);
// voor ways & relations geef een centrum terug
out center;

";


        using (var client = new HttpClient())
        {
            _logger.LogInformation("Prompt: {overpassQuery}", overpassQuery);
            string url = "https://overpass-api.de/api/interpreter?data=" 
                         + Uri.EscapeDataString(overpassQuery);
            var response = await client.GetAsync(url);


            response.EnsureSuccessStatusCode();

            // Parse JSON
            var json = await response.Content.ReadAsStringAsync();
            var root = JObject.Parse(json);
            var elements = (JArray)root["elements"];

            // Convert to list of JObject for easier handling
            var list = new List<JObject>();
            foreach (var el in elements)
                list.Add((JObject)el);

            var formattedList = new List<POI>();
            foreach (var poi in list)
            {
                // _logger.LogInformation("Response: {poi}", poi);
                var tags = poi["tags"] as JObject;
                string? highway = tags?["highway"]?.ToString();
                string category = "";
                if (highway == null)
                {
                    string? amenity = tags?["amenity"]?.ToString();
                    category = amenity;
                }
                else
                {
                    category = highway;
                }
                var poiObj = await GetOrCreatePoiAsync(db, new POI
                {
                    POIID = 0,
                    category = category,
                    osmId = poi["id"]?.Value<long>() ?? 0,
                    name = tags?["name"]
                        ?.ToString(),
                    latitude = poi["lat"]?.Value<double>() ?? 0.0,
                    longitude = poi["lon"]?.Value<double>() ?? 0.0,
                    source = "overpass",
                    retrievedAt = DateTime.Now,
                    detectionPOIs = new List<DetectionPOI>(),
                });
                formattedList.Add(poiObj);
            }

            return formattedList;
        }
    }
}