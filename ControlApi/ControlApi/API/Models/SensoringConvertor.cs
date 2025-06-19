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
            .Select(TempDetectionParser)
            .ToList();
    }
    
    /// <summary>
    /// Converts the full api response structure from the sensoring api into usable objects.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static List<TempDetection> ConvertFullModel(List<ApiTrashItem> data)
    {
        List<TempDetection> trashList = new List<TempDetection>();
        foreach (ApiTrashItem trashDet in data) 
        {
            trashList.Add(DetectionParser(trashDet));
        }

        return trashList;
    }

    /// <summary>
    /// the actual brain of this class.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static TempDetection TempDetectionParser(TestModeApiResponseData data)
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
    
    /// <summary>
    /// the actual brain of this class.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static TempDetection DetectionParser(ApiTrashItem data)
    {
        return new TempDetection()
        {
            actualTempC = data.actual_temp_celsius,
            confidence = data.confidence,
            trashType = data.type,
            feelsLikeTempC = data.feels_like_temp_celsius,
            latitude = data.latitude,
            longitude = data.longitude,
            timeStamp = DateTime.Parse(data.timestamp),
            windDirection = data.wind_direction,
            windForceBft = data.wind_force_bft
        };
    }
}