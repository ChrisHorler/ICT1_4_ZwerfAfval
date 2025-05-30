namespace ControlApi.Data.Entities;

public class User
{
    public int userId { get; set; }
    public string email { get; set; } = string.Empty;
    public string passwordHash { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
}