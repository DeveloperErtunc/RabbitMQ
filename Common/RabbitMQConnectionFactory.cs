using RabbitMQ.Client;

namespace Common;

public class RabbitMQConnectionFactory
{
    public static string QueueName = "hello-queue";
    public static ConnectionFactory GetRabbitMqConnection()
    {
        ConnectionFactory factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://localhost:5672");
        return factory;
    }
}
