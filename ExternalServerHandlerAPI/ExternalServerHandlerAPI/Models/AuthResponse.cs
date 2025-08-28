namespace ExternalServerHandlerAPI.Models;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? RequestId { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}
