using Domain.Common;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid ChatId { get; set; } // Идентификатор чата
    public Chat? Chat { get; set; } // Связь с отправителем

    public User? Sender { get; set; } // Отправитель сообщения
    public Guid SenderId { get; set; } // Идентификатор отправителя

    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
