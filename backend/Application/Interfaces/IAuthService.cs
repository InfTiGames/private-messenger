using Domain.Entities;

namespace Application.Interfaces;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string password);
    string GenerateJwtToken(User user);
}
