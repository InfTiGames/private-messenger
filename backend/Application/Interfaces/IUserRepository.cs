using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User?>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByNicknameAsync(string nickname);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task DeleteAllUsersAsync(); // Метод для удаления всех пользователей из таблицы Users
    Task DeleteUserAsync(string userId);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
}
