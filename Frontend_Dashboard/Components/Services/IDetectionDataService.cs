using Frontend_Dashboard.Components.Models;
using Frontend_Dashboard.Components.Services;

namespace Frontend_Dashboard.Components.Services
{
    public interface IDetectionDataService
    {
        Task<List<DetectionData>> GetDetectionDataAsync();
    }
}
