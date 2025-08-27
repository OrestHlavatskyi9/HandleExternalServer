namespace ExternalServerHandlerAPI.Models;

public class PACIConfig
{
    public string ServiceProviderId { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string CertificateThumbprint { get; set; } = string.Empty;
    public string PACIUrl { get; set; } = string.Empty;
}
