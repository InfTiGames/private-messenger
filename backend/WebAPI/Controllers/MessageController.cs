using Microsoft.AspNetCore.Mvc;
using Infrastructure.Messaging.Models;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageQueueService _messageQueueService;
    private readonly string _encryptionKey;

    public MessageController(IMessageQueueService messageQueueService, IConfiguration configuration)
    {
        _messageQueueService = messageQueueService;
        _encryptionKey =
            configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption key is not configured.");
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] string message)
    {
        // Шифруем сообщение
        var encryptedMessage = EncryptMessage(message);

        var messageDto = new MessageDto { Text = encryptedMessage };
        await _messageQueueService.SendMessageAsync("message-queue", messageDto);
        return Ok("Message sent successfully");
    }

    private string EncryptMessage(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
        aes.GenerateIV(); // Генерация случайного IV

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length); // Сохраняем IV в начале потока
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }
}
