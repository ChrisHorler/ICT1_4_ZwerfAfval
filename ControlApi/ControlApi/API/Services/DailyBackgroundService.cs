using ControlApi.API.DTOs;
using Newtonsoft.Json;

namespace ControlApi.SensoringConnection.Services;

public class DailyBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DailyBackgroundService> _logger;
    private readonly string _apiUrl;
    public DailyBackgroundService(
        IHttpClientFactory httpClientFactory, ILogger<DailyBackgroundService> logger, 
        IConfiguration config
        )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiUrl = config["SELF_API_BASE"]
                  ?? Environment.GetEnvironmentVariable("SELF_API_BASE")
                  ?? throw new InvalidOperationException("SELF_API_BASE not found");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool firstLoop = true;
        _logger.LogInformation("DailyGathering Service is running.");
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(15); // 18 PM
            if (now > targetTime)
            {
                if (firstLoop)
                {
                    await RunBackgroundTaskAsync(stoppingToken);
                }
                targetTime = targetTime.AddDays(1);
            }

            firstLoop = false;
            var delay = targetTime - now;
            Console.WriteLine(delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;
            
            await RunBackgroundTaskAsync(stoppingToken);
        }
    }

    private async Task RunBackgroundTaskAsync(CancellationToken stoppingToken)
    {
        this._logger.LogInformation("Updating our Trash Collection with the Sensoring API.");
        var client = _httpClientFactory.CreateClient();
        
        var response = await client.GetAsync(this._apiUrl, stoppingToken);
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync(stoppingToken);
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