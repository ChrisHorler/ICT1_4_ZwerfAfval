namespace ControlApi.Entities;

public class TrashType
{
    public int trashTypeId { get; set; }
    public string name { get; set; } = string.Empty;

    public ICollection<Detection> detections { get; set; } = new List<Detection>();
}