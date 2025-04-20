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

    public async Task DeleteAsync(User user) => _db.Users.Remove(user);

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}
