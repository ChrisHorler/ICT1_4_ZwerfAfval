using System.Net.Http.Json;
using Frontend_Dashboard.Components.Models;

public class CalendarService
{
    private readonly HttpClient _httpClient;

    public CalendarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<DateOnly, string>> GetPredictionsForMonthAsync(int year, int month)
    {
        var predictions = new Dictionary<DateOnly, string>();
        var daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(year, month, day);

            try
            {
                var response = await _httpClient.GetFromJsonAsync<CalendarPredictionDTO>(
                    $"api/PredictEndpoint/calendar?date={date:yyyy-MM-dd}");

                if (response != null)
                    predictions[date] = response.result.ToLower();
            }
            catch
            {
                // Fuck logging catch these hands
            }
        }
        return predictions;
    }
}