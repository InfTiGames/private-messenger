public interface IMessageQueueService
{
    /// <summary>
    /// Отправка сообщения в указанную очередь.
    /// </summary>
    /// <typeparam name="T">Тип сообщения.</typeparam>
    /// <param name="queueName">Имя очереди.</param>
    /// <param name="message">Сообщение для отправки.</param>
    Task SendMessageAsync<T>(string queueName, T message) where T : class;

    /// <summary>
    /// Подписка на указанную очередь.
    /// </summary>
    /// <typeparam name="T">Тип сообщения.</typeparam>
    /// <param name="queueName">Имя очереди.</param>
    /// <param name="handleMessage">Функция обработки сообщения.</param>
    void Subscribe<T>(string queueName, Func<T, Task> handleMessage) where T : class;
}
