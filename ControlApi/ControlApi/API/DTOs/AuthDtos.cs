using System.ComponentModel.DataAnnotations;

namespace ControlApi.API.DTOs;

public record RegisterRequest(
    [property: Required, EmailAddress] string email,
    [property: Required, MinLength(8),
               RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).+$",
                   ErrorMessage = "Password must contain an uppercase letter, lowercase letter, number, and symbol.")]
    string password
);
public record LoginRequest(string email, string password);
public record AuthResponse(string token, int userId,string email);