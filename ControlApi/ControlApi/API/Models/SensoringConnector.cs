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
                    List<TempDetection> trashDetections = SensoringConvertor.ConvertFullModel(parsedResponse);
                    // now populate it with locationdata
                    
                    // now populate it with prediction data
                    
                    
                    
                    
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
                _logger.LogWarning("Failed to fetch data from external API. Status code: {StatusCode}", response.StatusCode);
            }
        }
    }
}