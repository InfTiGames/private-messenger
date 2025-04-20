namespace WebAPI.Models.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public string Password { get; set; } = null!;
}
