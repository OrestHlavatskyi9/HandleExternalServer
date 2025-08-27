
using ExternalServerHandlerAPI.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace ExternalServerHandlerAPI.Services;

public class PACIService : IPACIService
{
    private readonly PACIConfig _config;

    public PACIService(IOptions<PACIConfig> config)
    {
        _config = config.Value;
    }
    public async Task<string> CallBackResponceAsync()
    {
        var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                _config.CertificateThumbprint,
                validOnly: false);

        if (certs.Count == 0)
        {
            throw new Exception("Certificate not found in store");
        }

        var cert = certs[0];

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(cert);

        using var client = new HttpClient(handler);

        var request = new
        {
            serviceProviderId = _config.ServiceProviderId,
            callbackUrl = _config.CallbackUrl
        };

        var response = await client.PostAsJsonAsync(
            _config.PACIUrl,
            request);

        return await response.Content.ReadAsStringAsync();

    }
}
