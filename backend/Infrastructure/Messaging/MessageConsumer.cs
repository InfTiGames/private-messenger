using MassTransit;

public class MessageConsumer : IConsumer<Message>
{
    public async Task Consume(ConsumeContext<Message> context)
    {
        var message = context.Message;
        Console.WriteLine($"Получено сообщение: {message.Content}");
        await Task.CompletedTask;
    }
}

public class Message
{
    public required string Content { get; set; }
}
