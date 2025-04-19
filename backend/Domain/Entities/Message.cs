using Domain.Common;

namespace Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }

        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
