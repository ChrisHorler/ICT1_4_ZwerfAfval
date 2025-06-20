public record DetectionDto(
    int detectionId,
    DateTime timeStamp,
    string trashType,
    double latitude,
    double longitude
);
public record LineGraphDto(
    int detectionId,
    DateTime timeStamp
);

public record BarchartGroupDto(
    string Name,
    string? Category,
    Dictionary<string, int> TrashTypeCounts
);
