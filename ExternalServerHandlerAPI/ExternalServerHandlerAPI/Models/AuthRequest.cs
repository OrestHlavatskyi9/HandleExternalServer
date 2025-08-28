using System.ComponentModel.DataAnnotations;

namespace ExternalServerHandlerAPI.Models;

public class AuthRequest
{
    public string CivilId { get; set; } = string.Empty;
}
