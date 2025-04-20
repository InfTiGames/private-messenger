using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;
    private readonly AppDbContext _dbContext;
    private readonly MessageCacheService _messageCacheService;

    public UserController(
        IUserRepository userRepo,
        IAuthService aut,
        ILogger<AuthController> logger,
        AppDbContext dbContext,
        MessageCacheService messageCacheService
    )
    {
        _userRepo = userRepo;
        _auth = aut;
        _logger = logger;
        _dbContext = dbContext;
        _messageCacheService = messageCacheService;
    }

    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userRepo.GetAllAsync();
        if (users == null || !users.Any())
            return NotFound("No users found");

        return Ok(
            users.Select(
                u =>
                    new
                    {
                        Id = u?.Id == null || u.Id == Guid.Empty ? Guid.NewGuid() : u.Id,
                        Email = u?.Email ?? string.Empty,
                        Nickname = u?.Nickname ?? string.Empty
                    }
            )
        );
    }

    [HttpDelete("deleteAllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAllUsers()
    {
        try
        {
            await _userRepo.DeleteAllUsersAsync();
            return Ok("All users deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all users");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("deleteUserData")]
    [Authorize]
    public async Task<IActionResult> DeleteUserData()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Пользователь не авторизован");

            // Удаление данных пользователя из базы данных
            var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return NotFound("Пользователь не найден");

            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();

            // Удаление сообщений из Redis
            await _messageCacheService.DeleteMessageAsync($"messages_{userId}");

            return Ok("Данные пользователя успешно удалены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении данных пользователя");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}
