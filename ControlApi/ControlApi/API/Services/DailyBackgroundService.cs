using ControlApi.API.Services;
using ControlApi.Data;
using ControlApi.SensoringConnection.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.SensoringConnection.Services;

public class DailyBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DailyBackgroundService> _logger;
    private readonly SensoringConnector _sensoringConnector;
    public DailyBackgroundService(
        IHttpClientFactory httpClientFactory, ILogger<DailyBackgroundService> logger, 
        ILogger<SensoringConnector> modelLogger,  IConfiguration config,
        IServiceScopeFactory scopeFactory
        )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _sensoringConnector= new SensoringConnector(_httpClientFactory, modelLogger, config, scopeFactory);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool firstLoop = true;
        _logger.LogInformation("DailyGathering Service is running.");
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(3); // 18 PM
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
        _logger.LogInformation("Running daily gathering.");
        await _sensoringConnector.PullAsync(stoppingToken);
    }
}