using Domain.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Admin";

    public List<ChatUser> ChatUsers { get; set; } = new(); // Связь с чатами через ChatUser
    public List<Message> Messages { get; set; } = new(); // Сообщения, отправленные пользователем
    public required ICollection<RefreshToken> RefreshTokens { get; set; } // Added navigation property
}
