var factory = RabbitMQConnectionFactory.GetRabbitMqConnection();

using (var connection = factory.CreateConnection())
{
    var channel = connection.CreateModel();
    channel.BasicQos(0, 1, false);
    var consumer = new EventingBasicConsumer(channel);
    channel.BasicConsume(RabbitMQConnectionFactory.QueueName, false, consumer);
    consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        channel.BasicAck(e.DeliveryTag,false);
        Console.WriteLine("Gelen Mesaj: " + message);
    };
    Console.ReadLine();
}
