using ExternalServerHandlerAPI.Models;

namespace ExternalServerHandlerAPI.Services;

public interface IPACIService
{
    Task<AuthResponse> SendAuthRequestAsync(string civilId);
    Task<string> ProcessCallbackAsync(object callbackData);
}
