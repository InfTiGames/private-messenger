using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Infrastructure.Messaging.Models;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Messaging;

public class MessageConsumer : IConsumer<MessageDto>
{
    private readonly ILogger<MessageConsumer> _logger;
    private readonly string _encryptionKey;

    public MessageConsumer(ILogger<MessageConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _encryptionKey =
            configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption key is not configured.");
    }

    public Task Consume(ConsumeContext<MessageDto> context)
    {
        // Расшифровываем сообщение
        var decryptedMessage = DecryptMessage(context.Message.Text);

        _logger.LogInformation("Received message: {Text}", decryptedMessage);
        return Task.CompletedTask;
    }

    private string DecryptMessage(string encryptedText)
    {
        var cipherBytes = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));

        var iv = new byte[16];
        Array.Copy(cipherBytes, 0, iv, 0, iv.Length); // Извлекаем IV из начала массива
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);

        return reader.ReadToEnd();
    }
}
