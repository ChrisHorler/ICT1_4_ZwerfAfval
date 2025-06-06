using ControlApi.Data;
using ControlApi.SensoringConnection.Models;

namespace ControlApi.SensoringConnection.Services;

public interface IDailyTaskExecutor
{
    Task ExecuteAsync(CancellationToken ct);
}

public class DailyTaskExecutor : IDailyTaskExecutor
{
    private readonly ControlApiDbContext _context;
    private readonly SensoringConnector _connector;

    public DailyTaskExecutor(ControlApiDbContext context, SensoringConnector connector)
    {
        _context = context;
        _connector = connector;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        await _connector.PullAsync(ct);
    }
}