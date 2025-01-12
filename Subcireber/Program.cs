using Newtonsoft.Json;

var factory = RabbitMQConnectionFactory.GetRabbitMqConnection();
//Basic mesaj cosume etme
//using (var connection = factory.CreateConnection())
//{
//    var channel = connection.CreateModel();
//    channel.BasicQos(0, 1, false);
//    var consumer = new EventingBasicConsumer(channel);
//    channel.BasicConsume(RabbitMQConnectionFactory.QueueName, false, consumer);
//    consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
//    {
//        var message = Encoding.UTF8.GetString(e.Body.ToArray());
//        channel.BasicAck(e.DeliveryTag,false);
//        Console.WriteLine("Gelen Mesaj: " + message);
//    };
//    Console.ReadLine();
//}
//Excangteki mesajı consume etme

//Console.WriteLine("uygulama çalışıyor");
//using (var connection = factory.CreateConnection())
//{
//    var channel = connection.CreateModel();
//    channel.BasicQos(0, 1, false);

//    var consumer = new EventingBasicConsumer(channel);

//    var randonQueueName = "data-base-save";

//    var Queue = channel.QueueDeclare(randonQueueName,true,false,false);
//    channel.QueueBind(Queue, RabbitMQConnectionFactory.FanoutExchangeName,"");
//    channel.BasicConsume(Queue, false, consumer);

//    consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
//    {
//        var message = Encoding.UTF8.GetString(e.Body.ToArray());
//        channel.BasicAck(e.DeliveryTag, false);
//        Console.WriteLine("Gelen Mesaj: " + message);
//    };
//    Console.ReadLine();
//}

//30sn bekletme
Console.WriteLine("Hava Durumu Raporu");
Console.WriteLine("------------------------------------");

using (var connection = factory.CreateConnection())
{
    string delayedExchangeName = "delayed-exchange"; // Gecikmeli exchange ismi
    var channel = connection.CreateModel();
    string QueueName = "delayed-queue";

    // Gecikmeli exchange'i tanımlıyoruz, "x-delayed-type" parametresini direct olarak ayarlıyoruz
    channel.ExchangeDeclare(delayedExchangeName, "x-delayed-message", true, false, new Dictionary<string, object>
    {
        { "x-delayed-type", "direct" }  // Gecikmeli mesajların yönlendirileceği exchange türü: direct
    });

    // Kuyruğu ve exchange'i bağlama
    channel.QueueDeclare(QueueName, true, false, false);
    channel.QueueBind(QueueName, delayedExchangeName, "");

    // Consumer'ı oluşturuyoruz
    var consumer = new EventingBasicConsumer(channel);
    channel.BasicConsume(QueueName, false, consumer);

    consumer.Received += (sender, e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        var weatherReport = JsonConvert.DeserializeObject<WeatherReport>(message);
        message = $"{weatherReport.DateTime} {WeatherReport.GetDescription(weatherReport.WeatherCondition)}";
        if(weatherReport.WeatherCondition == WeatherCondition.DisasterPossible)
        {
            // Mesajı tekrar kuyruğa 30 saniye gecikmeli olarak göndermek için x-delay başlığı ekliyoruz
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                { "x-delay", 10000 }  // 30 saniye gecikme (30000 ms)
            };

            // Mesajı 30 saniye gecikmeli olarak tekrar göndermek için BasicPublish kullanıyoruz
            channel.BasicPublish(delayedExchangeName, "", properties, e.Body);
        }
        Console.WriteLine(message);
        Console.WriteLine($"Mesajın Alındıgı Saat: {DateTime.Now.ToString("HH:mm:ss")}");
        Console.WriteLine("------------------------------------");

        // Mesajı başarıyla işlemi tamamladığımız için ACK işlemi yapıyoruz
        channel.BasicAck(e.DeliveryTag, false);
    };

    Console.ReadLine();
}