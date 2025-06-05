using ControlApi.API.DTOs;
using ControlApi.API.Services;
using ControlApi.Data;
using Newtonsoft.Json;

namespace ControlApi.SensoringConnection.Models;

public class SensoringConnector
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SensoringConnector> _logger;
    private readonly ControlApiDbContext _db;
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
    
    public async Task PullAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Updating our Trash Collection with the Sensoring API.");
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