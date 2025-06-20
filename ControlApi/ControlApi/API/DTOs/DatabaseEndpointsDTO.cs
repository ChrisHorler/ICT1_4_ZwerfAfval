namespace ControlApi.API.DTOs;
public record DetectionDto(
    int? detectionId,
    DateTime timeStamp,
    string trashType,
    double latitude,
    double longitude
);

public class BarchartDto
{
    public string Name { get; set; }
    public string Category { get; set; }
    public Dictionary<string, int> TrashTypeCounts { get; set; }
}

public record LineGraphDto(
    int? detectionId,
    DateTime timeStamp
);