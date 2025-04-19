namespace Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
