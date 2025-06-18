namespace ControlApi.API.DTOs;

public record FunFacts(
    string locationName,
    int totalTrashCount,
    string mostCommonTrashType
);

public record BarchartInfo(
    string poiName,
    Dictionary<string, int> trashTypeCounts // e.g. { "plastic bottles": 20, "cigarette booties": 40 }
);

public record LineGraphInfo(
    string date,
    int totalTrashCount
);