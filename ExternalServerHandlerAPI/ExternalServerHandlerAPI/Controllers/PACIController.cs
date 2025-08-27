using ExternalServerHandlerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExternalServerHandlerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PACIController : ControllerBase
{
    private readonly IPACIService _paciService;

    public PACIController(IPACIService paciService)
    {
        _paciService = paciService;
    }

    [HttpGet("CallBack")]
    public async Task<IActionResult> StartAuth()
    {
        var result = await _paciService.CallBackResponceAsync();
        return Ok(new { response = result });
    }

}
