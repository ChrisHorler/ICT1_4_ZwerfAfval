public record DetectionDto(
    int detectionId,
    DateTime timeStamp,
    string trashType,
    double? feelsLikeTempC,
    List<PoiDto>? pois = null 
);

// For POI data if needed
public record PoiDto(
    int POIID,
    string name,
    double latitude,
    double longitude
);

public record FilteredDetectionsResponse(
    List<DetectionDto> detections,
    int totalCount,
    DateTime? minDate = null,
    DateTime? maxDate = null
);