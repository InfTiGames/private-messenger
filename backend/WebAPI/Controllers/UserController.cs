using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly MessageCacheService _messageCacheService;

    public UserController(MessageCacheService messageCacheService)
    {
        _messageCacheService = messageCacheService;
    }

    [HttpDelete("delete-user-data")]
    public async Task<IActionResult> DeleteUserData(string userId)
    {
        // Удалите данные пользователя из Redis
        await _messageCacheService.DeleteAllMessagesForUserAsync(userId);

        return Ok("User data deleted successfully");
    }
}
