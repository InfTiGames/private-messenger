using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Infrastructure.Persistence.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _db;

    public ChatRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Chat?> GetByIdAsync(Guid chatId)
    {
        return await _db.Chats
            .Include(c => c.ChatUsers)
                .ThenInclude(cu => cu.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<IEnumerable<Chat>> GetAllAsync()
    {
        return await _db.Chats
            .Include(c => c.ChatUsers)
                .ThenInclude(cu => cu.User)
            .ToListAsync();
    }

    public async Task AddAsync(Chat chat)
    {
        await _db.Chats.AddAsync(chat);
        await _db.SaveChangesAsync();
    }

    public void Delete(Chat chat)
    {
        _db.Chats.Remove(chat);
    }

    public async Task AddUserToChatAsync(Guid chatId, Guid userId)
    {
        var chatUser = new ChatUser
        {
            ChatId = chatId,
            UserId = userId
        };

        await _db.ChatUsers.AddAsync(chatUser);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId)
    {
        var chatUser = await _db.ChatUsers
            .FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

        if (chatUser != null)
        {
            _db.ChatUsers.Remove(chatUser);
            await _db.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}