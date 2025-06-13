using Zwerfafval_WebApp.Components.Models;

namespace Zwerfafval_WebApp.Components.Services
{
    public interface IDetectionDataService
    {
        Task<List<DetectionData>> GetDetectionDataAsync();
    }
}
