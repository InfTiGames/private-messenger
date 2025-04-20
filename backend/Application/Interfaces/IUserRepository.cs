using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByNicknameAsync(string nickname);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}
