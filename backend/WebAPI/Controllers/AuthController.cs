using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Auth;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepo,
        IAuthService aut,
        ILogger<AuthController> logger
    )
    {
        _userRepo = userRepo;
        _auth = aut;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Попытка регистрации пользователя: {Email}", request.Email);
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Email already registered");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Nickname = request.Nickname,
                PasswordHash = _auth.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var token = _auth.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя: {Email}", request.Email);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        if (!_auth.VerifyPassword(user.PasswordHash, request.Password))
            return Unauthorized("Invalid credentials");

        var token = _auth.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMyProfile()
    {
        var userId = User?.Identity?.Name; // Или вытаскивай claim из токена
        return Ok(new { userId });
    }
}
