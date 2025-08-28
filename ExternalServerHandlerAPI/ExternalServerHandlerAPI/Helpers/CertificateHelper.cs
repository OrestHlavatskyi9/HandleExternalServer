using System.Security.Cryptography.X509Certificates;

namespace ExternalServerHandlerAPI.Helpers;

public static class CertificateHelper
{
    public static X509Certificate2 LoadFromStoreByThumbprint(string thumbprint)
    {
        if (string.IsNullOrWhiteSpace(thumbprint))
            throw new ArgumentException("Thumbprint is null or empty.");

        var normalized = thumbprint.Replace(" ", "").ToUpperInvariant();

        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        var matches = store.Certificates.Find(X509FindType.FindByThumbprint, normalized, false);

        if (matches == null || matches.Count == 0)
            throw new InvalidOperationException($"Certificate with thumbprint '{normalized}' not found in LocalMachine\\My.");

        var cert = matches.OfType<X509Certificate2>().First();
        if (!cert.HasPrivateKey)
            throw new InvalidOperationException($"Certificate with thumbprint '{normalized}' does not have a private key.");

        return cert;
    }
}
