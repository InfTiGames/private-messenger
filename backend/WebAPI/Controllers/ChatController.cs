using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatById(Guid chatId)
    {
        try
        {
            var chat = await _chatService.GetChatByIdAsync(chatId);
            return Ok(chat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat([FromBody] Chat chat)
    {
        try
        {
            await _chatService.CreateChatAsync(chat);
            return Ok("Chat created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{chatId}/addUser/{userId}")]
    public async Task<IActionResult> AddUserToChat(Guid chatId, Guid userId)
    {
        try
        {
            await _chatService.AddUserToChatAsync(chatId, userId);
            return Ok("User added to chat");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{chatId}/removeUser/{userId}")]
    public async Task<IActionResult> RemoveUserFromChat(Guid chatId, Guid userId)
    {
        try
        {
            await _chatService.RemoveUserFromChatAsync(chatId, userId);
            return Ok("User removed from chat");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
