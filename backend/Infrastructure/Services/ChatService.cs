using Application.Interfaces;
using Domain.Entities;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepo;
    private readonly IUserRepository _userRepo;

    public ChatService(IChatRepository chatRepo, IUserRepository userRepo)
    {
        _chatRepo = chatRepo;
        _userRepo = userRepo;
    }

    public async Task<Chat> GetChatByIdAsync(Guid chatId)
    {
        var chat = await _chatRepo.GetByIdAsync(chatId);
        if (chat == null)
            throw new KeyNotFoundException("Chat not found");

        return chat;
    }

    public async Task CreateChatAsync(Chat chat)
    {
        if (string.IsNullOrWhiteSpace(chat.Name))
            throw new ArgumentException("Chat name cannot be empty");

        await _chatRepo.AddAsync(chat);
    }

    public async Task AddUserToChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepo.GetByIdAsync(chatId);
        if (chat == null)
            throw new KeyNotFoundException("Chat not found");

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _chatRepo.AddUserToChatAsync(chatId, userId);
    }

    public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepo.GetByIdAsync(chatId);
        if (chat == null)
            throw new KeyNotFoundException("Chat not found");

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _chatRepo.RemoveUserFromChatAsync(chatId, userId);
    }
}
