namespace ControlApi.SensoringConnection.Services;

public class DailyGatheringService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DailyGatheringService> _logger;
    public DailyGatheringService(IHttpClientFactory httpClientFactory, ILogger<DailyGatheringService> logger)
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
                    _logger.LogInformation("Running daily gathering.");
                    // I'll add future logic here
                }
                targetTime = targetTime.AddDays(1);
            }

            firstLoop = false;
            var delay = targetTime - now;
            Console.WriteLine(delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;
            
            _logger.LogInformation("Running daily gathering.");
            // I'll add future logic here
        }
    }
}