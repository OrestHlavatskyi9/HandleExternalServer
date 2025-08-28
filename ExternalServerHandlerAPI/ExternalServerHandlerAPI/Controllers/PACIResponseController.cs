using ExternalServerHandlerAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExternalServerHandlerAPI.Controllers;

[ApiController]
[Route("PACIMarkaz/api")]
public class PACIResponseController : ControllerBase
{
    private readonly IPACIService _paciService;
    private readonly ILogger<PACIResponseController> _logger;

    public PACIResponseController(IPACIService paciService, ILogger<PACIResponseController> logger)
    {
        _paciService = paciService;
        _logger = logger;
    }

    /// <summary>
    /// Callback endpoint for PACI responses
    /// </summary>
    [HttpPost("PACIResponse")]
    public async Task<ActionResult<string>> PACIResponse([FromBody] object callbackData)
    {
        try
        {
            _logger.LogInformation("PACI callback received");

            var processedData = await _paciService.ProcessCallbackAsync(callbackData);

            return Ok(processedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PACI callback");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
