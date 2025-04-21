namespace Application.DTOs;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Nickname { get; set; }
    public required string Password { get; set; }
}
