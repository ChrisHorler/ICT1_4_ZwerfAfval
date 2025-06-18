namespace ControlApi.API.DTOs;

public record TestModeApiTrashItem(
    string timestamp, string type,
    float confidence, float? longitude, float? latitude,
    float feels_like_temp_celsius,
    float actual_temp_celsius, float wind_force_bft,
    float wind_direction
    );

public record TestModeApiResponseData(
    string _id, string projectId,
    int clearanceLevelNeeded, int __v,
    TestModeApiTrashItem attributes
    ); // can be removed once we have the real api
    
public record TestModeApiResponse(List<TestModeApiResponseData> projectData);