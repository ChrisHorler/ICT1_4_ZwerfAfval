namespace ControlApi.SensoringConnection.Services;

public class DailyGatheringService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var targetTime = DateTime.Today.AddHours(18); // 18 PM
            if (now > targetTime)
            {
                targetTime = targetTime.AddDays(1);
            }
            var delay = targetTime - now;
            Console.WriteLine(delay);
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            Console.WriteLine("Running daily gathering");
            // i'll add future logic here
        }
    }
}