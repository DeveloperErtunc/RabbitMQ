ConnectionFactory factory = RabbitMQConnectionFactory.GetRabbitMqConnection();

using(var connection = factory.CreateConnection())
{
    var channel = connection.CreateModel();
    channel.QueueDeclare(RabbitMQConnectionFactory.QueueName,true, false, false);
    Enumerable.Range(1, 100).ToList().ForEach(x =>
    {
        var message = $"Message {x}";
        var bytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(string.Empty, RabbitMQConnectionFactory.QueueName, null, bytes);
        Console.WriteLine($"Mesaj Gönderildi : {message}");
    });
    Console.ReadLine();
}
