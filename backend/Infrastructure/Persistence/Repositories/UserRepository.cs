using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByNicknameAsync(string nickname) =>
        await _db.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

    public async Task<User?> GetByIdAsync(Guid id) => await _db.Users.FindAsync(id);

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();

    public async Task<IEnumerable<User?>> GetAllAsync()
    {
        return await _db.Users.ToListAsync(); // Получение всех пользователей из таблицы Users
    }

    public async Task DeleteAllUsersAsync()
    {
        var users = await _db.Users.ToListAsync(); // Получение всех пользователей из таблицы Users
        _db.Users.RemoveRange(users); // Удаление всех пользователей
        await _db.SaveChangesAsync(); // Сохранение изменений в базе данных
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        return await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken); // Поиск токена в таблице RefreshTokens
    }
}
