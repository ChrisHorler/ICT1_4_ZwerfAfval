namespace ControlApi.SensoringConnection.Services;

public class DailyBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DailyBackgroundService> _logger;
    public DailyBackgroundService(IHttpClientFactory httpClientFactory, ILogger<DailyBackgroundService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool firstLoop = true;
        _logger.LogInformation("DailyGathering Service is running.");
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(18); // 18 PM
            if (now > targetTime)
            {
                if (firstLoop)
                {
                    await RunBackgroundAsync(stoppingToken);
                }
                targetTime = targetTime.AddDays(1);
            }

            firstLoop = false;
            var delay = targetTime - now;
            Console.WriteLine(delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;
            
            await RunBackgroundAsync(stoppingToken);
        }
    }

    private async Task RunBackgroundAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Running daily gathering.");
    }
}