namespace Frontend_Dashboard.Components.Models;

public class LoginDto
{
    public string email    { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}

public class RegisterDto : LoginDto { } 

public record AuthResponse(string token, int userId, string email);