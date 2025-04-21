using Domain.Entities;

namespace Application.Interfaces;

public interface IChatService
{
    Task<Chat> GetChatByIdAsync(Guid chatId);
    Task CreateChatAsync(Chat chat);
    Task AddUserToChatAsync(Guid chatId, Guid userId);
    Task RemoveUserFromChatAsync(Guid chatId, Guid userId);
}
