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
    private bool _failedResponse = false;
    private int _failedResponses = 0;
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
            if (this._failedResponse)
            {
                if (this._failedResponses >= 5)
                {
                    this._logger.LogWarning($"Retried {this._failedResponses} times, skipping today...");
                    this._failedResponse = false;
                    this._failedResponses = 0;
                    continue;
                }
                this._logger.LogInformation("Retrying to fetch...");
                this._failedResponse = false;
                TimeSpan oneMinute = TimeSpan.FromMinutes(1);
                await Task.Delay(oneMinute, stoppingToken);
                
                await RunBackgroundTaskAsync(stoppingToken);
                continue;
            }

            this._failedResponses = 0;
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(18); // 18 PM
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
        this._logger.LogInformation("Running daily gathering.");
        try
        {
            await _sensoringConnector.PullAsync(stoppingToken);
        }
        catch (DataException excpt)
        {
            this._logger.LogWarning($"Error whilst calling SensoringAPI: {excpt}");
            this._failedResponse = true;
            this._failedResponses += 1;
        }
    }
}