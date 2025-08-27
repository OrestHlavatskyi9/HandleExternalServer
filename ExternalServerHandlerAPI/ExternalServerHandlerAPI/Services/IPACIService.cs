namespace ExternalServerHandlerAPI.Services;

public interface IPACIService
{
    Task<string> CallBackResponceAsync();
}
