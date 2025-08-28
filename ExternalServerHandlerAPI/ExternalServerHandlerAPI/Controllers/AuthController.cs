using ExternalServerHandlerAPI.Models;
using ExternalServerHandlerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalServerHandlerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPACIService _paciService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IPACIService paciService, ILogger<AuthController> logger)
    {
        _paciService = paciService;
        _logger = logger;
    }

    /// <summary>
    /// Send authentication request to PACI service
    /// </summary>
    /// <param name="request">Civil ID for authentication</param>
    /// <returns>Authentication response</returns>
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Authentication request received for Civil ID: {request.CivilId}");

        var result = await _paciService.SendAuthRequestAsync(request.CivilId);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
