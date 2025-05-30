namespace ControlApi.API.DTOs;

public record RegisterRequest(string email, string password);
public record LoginRequest(string email, string password);
public record AuthResponse(string token, int userId,string email);