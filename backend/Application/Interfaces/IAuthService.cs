using Domain.Entities;
using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    // Работа с паролями
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string password);

    // Работа с токенами
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();

    // Регистрация и вход
    Task<AuthResponse> RegisterUserAsync(RegisterRequest request);
    Task<AuthResponse> LoginUserAsync(LoginRequest request);

    // Обновление токенов
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    // Получение профиля
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
}
