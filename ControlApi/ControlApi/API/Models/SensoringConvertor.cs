using ControlApi.API.DTOs;
using ControlApi.Data.Entities;

namespace ControlApi.SensoringConnection.Models;

public static class SensoringConvertor
{
    public static List<TempDetection> ConvertFullModel(ApiResponse data)
    {
        return data.projectData
            .Select(DetectionParser)
            .ToList();
    }

    private static TempDetection DetectionParser(ApiResponseData data)
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