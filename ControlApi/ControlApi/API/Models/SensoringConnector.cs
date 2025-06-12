using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.Data.Entities;
using Newtonsoft.Json;

namespace ControlApi.SensoringConnection.Models;

public class SensoringConnector
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SensoringConnector> _logger;
    private ControlApiDbContext? _db;
    private readonly IJwtService _jwt;
    private readonly string _apiUrl;

    public SensoringConnector(
        IHttpClientFactory httpClientFactory, 
        ILogger<SensoringConnector> logger, 
        IConfiguration config
        )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiUrl = config["SENSORING_API"]
                  ?? Environment.GetEnvironmentVariable("SENSORING_API")
                  ?? throw new InvalidOperationException("SENSORING_API not found");
        
    }

    public void SetDbContext(ControlApiDbContext db)
    {
        this._logger.LogInformation("DbContext Provided to SensoringConnector");
        this._db = db;
    }

    public async Task PullAsync(CancellationToken cancellationToken)
    {
        if (this._db == null)
        {
            this._logger.LogInformation("Tried to pull data from Sensoring API without DbContext.");
        }

        var latestItem = this._db.detections
            .OrderByDescending(e => e.timeStamp)
            .FirstOrDefault();
        if (latestItem == null)
        {
            latestItem = new Detection();
            latestItem.timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        this._logger.LogInformation("Updating our Trash Collection with the Sensoring API.\nLastUpdate: {item}", latestItem.timeStamp);
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync(this._apiUrl, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                ApiResponse parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(data);
                _logger.LogInformation("Received data from external API: {parsedResponse}", data);
            }
            catch (JsonException exception)
            {
                _logger.LogError("Received data from external API, it is NOT Deserializable: {Data}", data);
            }
        }
        else
        {
            _logger.LogWarning("Failed to fetch data from external API. Status code: {StatusCode}", response.StatusCode);
        }
    }
}