using Domain.Common;

namespace Domain.Entities;

public class Chat : BaseEntity
{
    public List<User> Participants { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
}
