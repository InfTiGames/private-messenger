using Domain.Entities;

namespace Application.Interfaces;

public interface IChatRepository
{
    Task<Chat?> GetByIdAsync(Guid chatId);
    Task<IEnumerable<Chat>> GetAllAsync();
    Task AddAsync(Chat chat);
    void Delete(Chat chat);
    Task AddUserToChatAsync(Guid chatId, Guid userId);
    Task RemoveUserFromChatAsync(Guid chatId, Guid userId);
    Task SaveChangesAsync();
}
