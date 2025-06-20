namespace Frontend_Dashboard.Components.Models;

public class LineGraphDto
{
    public string? name { get; set; }
    public string category { get; set; } = string.Empty;
    public Dictionary<string, int> TrashTypeCounts { get; set; }
}

// example of a record:
// public record BarChartDTO(string? name, string category, Dictionary<string, int> TrashTypeCounts);