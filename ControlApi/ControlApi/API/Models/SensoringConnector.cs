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
    
    const int DETECTION_RADIUS = 50;

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
        // We need to load all this inside the scope otherwise we cant use the dbcontext
        // and no we can't use dependency injection for the dbcontext, because we are running this from a BackgroundService.
        using (var scope = _scopeFactory.CreateScope())
        {
            // load the dbcontext from the scope
            var dbContext = scope.ServiceProvider.GetRequiredService<ControlApiDbContext>();
            if (dbContext == null)
            {
                this._logger.LogInformation("Tried to pull data from Sensoring API without DbContext.");
                return;
            }

            // this will be used again later on
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

            // THIS IS THE QUERY TO THE API URL.
            // ONCE WE CHANGE THE API FROM MY PRIVATE API TO THE ACTUAL SENSORING ONE, WE'LL HAVE TO CHANGE A BIT OF CODE BELOW.
            // luckily most is handled in 2 classes, `SensoringApiDtos.cs` + `SensoringConvertor.cs`
            var response = await client.GetAsync(this._apiUrl, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    // this should still work when the api changes if I were to change the Dtos correctly.
                    ApiResponse parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(data); 
                    
                    _logger.LogInformation("Received data from external API: {parsedResponse}", data);
                    List<TempDetection> trashDetections = SensoringConvertor.ConvertFullModel(parsedResponse);
                    List<Detection> trashDets = new List<Detection>();
                    _logger.LogInformation("Parsed data to correct format: {trashDetections}", trashDetections);
                    foreach (var trashDetection in trashDetections)
                    {
                        if (await dbContext.detections.AnyAsync(d => 
                                d.timeStamp.Date <= latestItem.timeStamp))
                        {
                            continue;
                        }
                        var responseObj = await this.QueryNearbyElementsAsync(trashDetection.latitude, trashDetection.longitude, DETECTION_RADIUS, dbContext);
                        _logger.LogInformation("Response: {responseObj}", responseObj);
                        Detection det = trashDetection.ConvertToDetection();
                        foreach (var poiObj in responseObj)
                        {
                            det.detectionPOIs.Add(new DetectionPOI()
                            {
                                POIID = poiObj.POIID,
                                detectionRadiusM = DETECTION_RADIUS
                            });
                        }
                        trashDets.Add(det);
                    }
                    dbContext.detections.AddRange(trashDets);
                    dbContext.SaveChanges();
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
    
    private async Task<POI> GetOrCreatePoiAsync(ControlApiDbContext db, POI newPoi)
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
    /// Queries the Overpass API for restaurants, bus stops, etc
    /// </summary>
    private async Task<List<POI>> QueryNearbyElementsAsync(double lat, double lon, int radius, ControlApiDbContext db)
    {
        // following here is the massive ass query that our api requires.
        string overpassQuery = $@"
[out:json][timeout:25];
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

  // Ice‑cream shops & gelaterias
  node[shop=ice_cream](around:{radius},{lon},{lat});
  way[shop=ice_cream](around:{radius},{lon},{lat});
  relation[shop=ice_cream](around:{radius},{lon},{lat});

  node[shop=bakery](around:{radius},{lon},{lat});
  way[shop=bakery](around:{radius},{lon},{lat});
  relation[shop=bakery](around:{radius},{lon},{lat});
  
  node[shop=butcher](around:{radius},{lon},{lat});
  way[shop=butcher](around:{radius},{lon},{lat});
  relation[shop=butcher](around:{radius},{lon},{lat});
  
  node[shop=greengrocer](around:{radius},{lon},{lat});
  way[shop=greengrocer](around:{radius},{lon},{lat});
  relation[shop=greengrocer](around:{radius},{lon},{lat});
  
  node[shop=delicatessen](around:{radius},{lon},{lat});
  way[shop=delicatessen](around:{radius},{lon},{lat});
  relation[shop=delicatessen](around:{radius},{lon},{lat});
  
  // Supermarkets & convenience stores
  node[shop=supermarket](around:{radius},{lon},{lat});
  way[shop=supermarket](around:{radius},{lon},{lat});
  relation[shop=supermarket](around:{radius},{lon},{lat});
  
  node[shop=convenience](around:{radius},{lon},{lat});
  way[shop=convenience](around:{radius},{lon},{lat});
  relation[shop=convenience](around:{radius},{lon},{lat});

  node[amenity=marketplace](around:{radius},{lon},{lat});
  way[amenity=marketplace](around:{radius},{lon},{lat});
  relation[amenity=marketplace](around:{radius},{lon},{lat});
);
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

            // Convert to list of JObject for easier handling (this is like JSON equivalent in c#, but then with typing).
            var list = new List<JObject>();
            foreach (var el in elements)
                list.Add((JObject)el);

            var formattedList = new List<POI>();
            foreach (var poi in list)
            {
                var tags = poi["tags"] as JObject;

                string category = tags?["highway"]?.ToString()
                                  ?? tags?["amenity"]?.ToString()
                                  ?? tags?["shop"]?.ToString()
                                  ?? "";

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