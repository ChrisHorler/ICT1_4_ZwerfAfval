using ControlApi.API.DTOs;
using ControlApi.Data.Entities;
using Newtonsoft.Json;

namespace ControlApi.SensoringConnection.Models;

public interface ISensoringDataGrabber
{
    Task<List<TempDetection>> HandleAndConvert(HttpResponseMessage response, CancellationToken cancellationToken, ILogger<SensoringConnector> _logger);
}

public class TestModeSenseringDataGrabber : ISensoringDataGrabber {
    public async Task<List<TempDetection>> HandleAndConvert(HttpResponseMessage response, CancellationToken cancellationToken, ILogger<SensoringConnector> _logger)
    {
        string data = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            TestModeApiResponse parsedResponse = JsonConvert.DeserializeObject<TestModeApiResponse>(data);

            _logger.LogInformation("Received data from external API: {parsedResponse}", data);
            List<TempDetection> trashDetections = SensoringConvertor.ConvertFullModel(parsedResponse);
            return trashDetections;
        }
        catch (JsonException exception) {
            _logger.LogError("Received data from external API, it is NOT Deserializable: {Data}", data);
            return new List<TempDetection>();
        }
    }
}

public class ActualSenseringDataGrabber : ISensoringDataGrabber {
    public async Task<List<TempDetection>> HandleAndConvert(HttpResponseMessage response, CancellationToken cancellationToken, ILogger<SensoringConnector> _logger)
    {
        string data = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            List<ApiTrashItem> parsedResponse = JsonConvert.DeserializeObject<List<ApiTrashItem>>(data);
            _logger.LogInformation("Received data from external API: {parsedResponse}", data);
            List<TempDetection> trashDetections = SensoringConvertor.ConvertFullModel(parsedResponse);
            return trashDetections;
        }
        catch (JsonException exception) {
            _logger.LogError("Received data from external API, it is NOT Deserializable: {Data}", data);
            return new List<TempDetection>();
        }
    }
}