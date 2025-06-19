using Zwerfafval_WebApp.Components.Models;
using Zwerfafval_WebApp.Components.Services;

namespace Zwerfafval_WebApp.Components.Services
{
    public interface IDetectionDataService
    {
        Task<List<DetectionData>> GetDetectionDataAsync();
    }
}
