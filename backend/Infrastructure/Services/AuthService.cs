using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nickname)
        };

        var jwtKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<AuthResponse> RegisterUserAsync(RegisterRequest request)
    {
        if (await _userRepository.GetByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            Email = request.Email,
            Nickname = request.Nickname,
            PasswordHash = HashPassword(request.Password),
            RefreshTokens = new List<RefreshToken>() // Initialize RefreshTokens as an empty list
        };

        await _userRepository.AddAsync(user);

        return new AuthResponse
        {
            AccessToken = GenerateJwtToken(user),
            RefreshToken = GenerateRefreshToken()
        };
    }

    public async Task<AuthResponse> LoginUserAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !VerifyPassword(user.PasswordHash, request.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponse
        {
            AccessToken = GenerateJwtToken(user),
            RefreshToken = GenerateRefreshToken()
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _userRepository.GetRefreshTokenAsync(refreshToken);
        if (tokenEntity == null || tokenEntity.Expires < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        return new AuthResponse
        {
            AccessToken = GenerateJwtToken(user),
            RefreshToken = GenerateRefreshToken()
        };
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Nickname = user.Nickname
        };
    }
}
