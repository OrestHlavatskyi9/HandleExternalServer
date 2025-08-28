
using ExternalServerHandlerAPI.Helpers;
using ExternalServerHandlerAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace ExternalServerHandlerAPI.Services;

public class PACIService : IPACIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PACIService> _logger;

    // Constants from your configuration
    private const string SERVICE_PROVIDER_ID = "bbf07d6d-6842-4c2c-840c-cd50c9f46769";
    private const string CERT_THUMBPRINT = "cf55d2b6702211bc6dcc3a4ed433cfa06b41b7b5";
    private const string PACI_SERVICE_URL = "https://mid-auth-t.paci.gov.kw:5869";
    private const string CALLBACK_URL = "https://crmid.markaz.com/PACIMarkaz/api/PACIResponse";

    public PACIService(HttpClient httpClient, ILogger<PACIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AuthResponse> SendAuthRequestAsync(string civilId)
    {
        try
        {
            _logger.LogInformation($"Sending authentication request for Civil ID: {civilId}");

            // Get client certificate
            var clientCert = GetClientCertificate();
            if (clientCert == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Client certificate not found"
                };
            }

            // Create request payload
            var requestPayload = new
            {
                ServiceProviderId = SERVICE_PROVIDER_ID,
                PersonCivilNo = civilId,
                SPCallbackURL = CALLBACK_URL,
                ServiceDescriptionEN = "Authentication Service",
                ServiceDescriptionAR = "خدمة المصادقة",
                AuthenticationReasonEn = "User Authentication",
                AuthenticationReasonAr = "مصادقة المستخدم",
                RequestUserDetails = true
            };

            // Configure HttpClient with certificate
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCert);
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            using var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending request to: {PACI_SERVICE_URL}");
            _logger.LogInformation($"Request payload: {jsonContent}");

            // Send request to PACI
            var response = await httpClient.PostAsync(PACI_SERVICE_URL, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"PACI Response Status: {response.StatusCode}");
            _logger.LogInformation($"PACI Response: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                return new AuthResponse
                {
                    Success = true,
                    RequestId = Guid.NewGuid().ToString(),
                    Message = "Authentication request sent successfully",
                    Data = responseContent
                };
            }
            else
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"PACI API Error: {response.StatusCode} - {responseContent}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending authentication request");
            return new AuthResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<string> ProcessCallbackAsync(object callbackData)
    {
        try
        {
            _logger.LogInformation("Processing PACI callback");

            var jsonData = System.Text.Json.JsonSerializer.Serialize(callbackData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            _logger.LogInformation($"Callback Data: {jsonData}");

            // Here you can process the callback data
            // Parse it, validate it, store it in database, etc.

            return jsonData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing callback");
            throw;
        }
    }

    private X509Certificate2? GetClientCertificate()
    {
        try
        {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                CERT_THUMBPRINT,
                false);

            if (certificates.Count > 0)
            {
                _logger.LogInformation($"Certificate found: {certificates[0].Subject}");
                return certificates[0];
            }

            _logger.LogError($"Certificate with thumbprint {CERT_THUMBPRINT} not found in LocalMachine/My store");

            // Try Current User store as fallback
            using var userStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            userStore.Open(OpenFlags.ReadOnly);

            var userCertificates = userStore.Certificates.Find(
                X509FindType.FindByThumbprint,
                CERT_THUMBPRINT,
                false);

            if (userCertificates.Count > 0)
            {
                _logger.LogInformation($"Certificate found in CurrentUser store: {userCertificates[0].Subject}");
                return userCertificates[0];
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading client certificate");
            return null;
        }
    }
}
