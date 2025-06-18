using System.Data;
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
    private readonly bool _isTest;
    public DailyBackgroundService(
        IHttpClientFactory httpClientFactory, ILogger<DailyBackgroundService> logger, 
        ILogger<SensoringConnector> modelLogger,  IConfiguration config,
        IServiceScopeFactory scopeFactory
        )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _sensoringConnector= new SensoringConnector(_httpClientFactory, modelLogger, config, scopeFactory);
        _isTest = config.GetValue<bool>("Testing");
        if (_isTest)
        {
            _logger.LogWarning("The backend is running in TEST MODE. Is this correct? Read setup.md to change this.");
        }
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
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;
            
            await RunBackgroundTaskAsync(stoppingToken);
        }
    }

    private async Task RunBackgroundTaskAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Running daily gathering.");
        try
        {
            await _sensoringConnector.PullAsync(stoppingToken);
        }
        catch (DataException excpt)
        {
            _logger.LogWarning($"Error whilst calling SensoringAPI: {excpt}");
        }
    }
}