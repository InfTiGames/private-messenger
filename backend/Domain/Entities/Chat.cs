using Domain.Common;

namespace Domain.Entities;

public class Chat : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public List<ChatUser> ChatUsers { get; set; } = new(); // Связь с пользователями через ChatUser
    public List<Message> Messages { get; set; } = new(); // Сообщения в чате
}
