using Frontend_Dashboard.Components.Models;
using Frontend_Dashboard.Components.Services;

namespace Frontend_Dashboard.Components.Services
{
    public interface IAnalyticsDataService
    {
        Task<List<BarChartDto>> GetBarChartDataAsync(DateOnly date);
        Task<List<LineGraphDto>> GetLineGraphDataAsync(DateOnly from, DateOnly to);
    }
}
