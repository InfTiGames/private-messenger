using Domain.Entities;

namespace Domain.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetMessagesForChatAsync(Guid chatId);
}
