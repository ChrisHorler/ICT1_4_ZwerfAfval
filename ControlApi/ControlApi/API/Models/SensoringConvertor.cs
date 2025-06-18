using ControlApi.API.DTOs;
using ControlApi.Data.Entities;

namespace ControlApi.SensoringConnection.Models;

public static class SensoringConvertor
{
    /// <summary>
    /// Converts the full api response structure from the sensoring api into usable objects.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static List<TempDetection> ConvertFullModel(TestModeApiResponse data)
    {
        return data.projectData
            .Select(DetectionParser)
            .ToList();
    }

    /// <summary>
    /// the actual brain of this class.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static TempDetection DetectionParser(TestModeApiResponseData data)
    {
        return new TempDetection()
        {
            actualTempC = data.attributes.actual_temp_celsius,
            confidence = data.attributes.confidence,
            trashType = data.attributes.type,
            feelsLikeTempC = data.attributes.feels_like_temp_celsius,
            latitude = data.attributes.latitude,
            longitude = data.attributes.longitude,
            timeStamp = DateTime.Parse(data.attributes.timestamp),
            windDirection = data.attributes.wind_direction,
            windForceBft = data.attributes.wind_force_bft
        };
    }
}