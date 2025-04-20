using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IAuthService> _logger;

    public AuthService(IConfiguration configuration, ILogger<IAuthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hashed = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: 32
        );

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hashed)}";
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var attemptedHash = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA512,
            100000,
            32
        );

        return hash.SequenceEqual(attemptedHash);
    }

    public string GenerateJwtToken(User user)
    {
        _logger.LogInformation("Generating JWT token for user: {UserId}", user.Nickname);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nickname),
        };

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            _logger.LogError("JWT key is not configured. Please set Jwt:Key in appsettings.json.");
            throw new InvalidOperationException("JWT key is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"], // IMPORTANT!!! with "Jwt"
            audience: _configuration["Jwt:Audience"], // IMPORTANT!!! with "Jwt"
            claims: claims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: creds
        );

        _logger.LogInformation(
            "JWT token generated successfully for user: {UserId}",
            user.Nickname
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
