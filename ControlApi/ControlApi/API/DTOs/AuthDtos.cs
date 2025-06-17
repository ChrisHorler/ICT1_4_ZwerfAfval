using System.ComponentModel.DataAnnotations;

namespace ControlApi.API.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Invalid E-mail or Password")]
    public string password { get; init; } = string.Empty;
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string password { get; init; } = string.Empty;   
}
public record AuthResponse(string token, int userId,string email);