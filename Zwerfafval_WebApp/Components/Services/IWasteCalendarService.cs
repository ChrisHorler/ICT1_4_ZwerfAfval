using Zwerfafval_WebApp.Components.Models;
using Zwerfafval_WebApp.Components.Services;

namespace Zwerfafval_WebApp.Components.Services
{
    public interface IWasteCalendarService
    {
        Task<List<WasteCalendarData>> GetDetectionDataAsync();
    }
}
