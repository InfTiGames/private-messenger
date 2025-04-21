using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class AuthResponse
{
    [Required]
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
