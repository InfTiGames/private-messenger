using Domain.Common;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Storage only password hash for security

        public List<Chat> Chats { get; set; } = [];
    }
}
