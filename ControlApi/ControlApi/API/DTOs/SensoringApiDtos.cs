namespace ControlApi.API.DTOs;

public record ApiTrashItem(
    string timestamp, string type,
    float confidence, float longitude, float latitude,
    float feels_like_temp_celsius,
    float actual_temp_celsius, float wind_force_bft,
    float wind_direction
    );

public record ApiResponseData(
    string _id, string projectId,
    int clearanceLevelNeeded, int __v,
    ApiTrashItem attributes
    ); // can be removed once we have the real api
    
public record ApiResponse(List<ApiResponseData> projectData);