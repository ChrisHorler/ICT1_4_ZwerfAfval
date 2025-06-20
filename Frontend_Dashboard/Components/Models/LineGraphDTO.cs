namespace Frontend_Dashboard.Components.Models;

public class LineGraphDto
{
    public string? name { get; set; }
    public string category { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
    public Dictionary<string, int> TrashTypeCounts { get; set; }
}
