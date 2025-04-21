namespace Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Nickname { get; set; }
}
