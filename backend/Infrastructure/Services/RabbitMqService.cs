using MassTransit;
using Microsoft.Extensions.Logging;

public class RabbitMqService : IMessageQueueService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IBusControl busControl, ILogger<RabbitMqService> logger)
    {
        _busControl = busControl;
        _logger = logger;
    }

    /// <summary>
    /// Отправка сообщения в указанную очередь.
    /// </summary>
    /// <param name="queueName">Имя очереди.</param>
    /// <param name="message">Сообщение для отправки.</param>
    public async Task SendMessageAsync<T>(string queueName, T message) where T : class
    {
        try
        {
            var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await sendEndpoint.Send(message);

            _logger.LogInformation("Message sent to queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to queue {QueueName}", queueName);
            throw;
        }
    }

    /// <summary>
    /// Подписка на указанную очередь.
    /// </summary>
    /// <typeparam name="T">Тип сообщения.</typeparam>
    /// <param name="queueName">Имя очереди.</param>
    /// <param name="handleMessage">Функция обработки сообщения.</param>
    public void Subscribe<T>(string queueName, Func<T, Task> handleMessage) where T : class
    {
        try
        {
            _logger.LogInformation("Subscribing to queue {QueueName}", queueName);

            _busControl.ConnectReceiveEndpoint(
                queueName,
                endpoint =>
                {
                    endpoint.Handler<T>(async context =>
                    {
                        try
                        {
                            await handleMessage(context.Message);
                            _logger.LogInformation(
                                "Message processed from queue {QueueName}",
                                queueName
                            );
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Error processing message from queue {QueueName}",
                                queueName
                            );
                        }
                    });
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to queue {QueueName}", queueName);
            throw;
        }
    }
}
