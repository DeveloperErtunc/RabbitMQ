using Common;
using Newtonsoft.Json;

ConnectionFactory factory = RabbitMQConnectionFactory.GetRabbitMqConnection();

//Basic gönderme

//using(var connection = factory.CreateConnection())
//{
//    var channel = connection.CreateModel();
//    channel.QueueDeclare(RabbitMQConnectionFactory.QueueName,true, false, false);
//    Enumerable.Range(1, 100).ToList().ForEach(x =>
//    {
//        var message = $"Message {x}";
//        var bytes = Encoding.UTF8.GetBytes(message);
//        channel.BasicPublish(string.Empty, RabbitMQConnectionFactory.QueueName, null, bytes);
//        Console.WriteLine($"Mesaj Gönderildi : {message}");
//    });
//    Console.ReadLine();   
//}

//long-fanout ile gönderme kuyruk oluşturmadan
using (var connection = factory.CreateConnection())
{
    string delayedExchangeName = "delayed-exchange"; // Gecikmeli exchange ismi
    string type = "x-delayed-message";
    var channel = connection.CreateModel();

    // Gecikmeli exchange'i tanımlıyoruz, "x-delayed-type" parametresini fanout olarak ayarlıyoruz
    channel.ExchangeDeclare(delayedExchangeName, type, true, false, new Dictionary<string, object>
    {
        { "x-delayed-type", "fanout" }  // Gecikmeli mesajların yönlendirileceği exchange türü: fanout
    });
    // Kuyruğu tanımlıyoruz
    channel.QueueDeclare("", true, false, false);
    // Kuyruğu ve exchange bağlamayı yapıyoruz
    channel.QueueBind("", delayedExchangeName, "");


    var intValues = Enum.GetValues(typeof(WeatherCondition))
                              .Cast<int>()
                              .ToList();
    // Gün, ay ve yıl formatında dönüştürme
    DateTime currentDate = DateTime.Now;


    // Mesajları gönderiyoruz
    foreach (var item in intValues)
    {
        string formattedDate = currentDate.ToString("dd/MM/yyyy");
        var message = new WeatherReport(formattedDate, (WeatherCondition)item);
        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        channel.BasicPublish(delayedExchangeName, "", null, bytes);
        Console.WriteLine($"Mesaj  :{message.DateTime} {WeatherReport.GetDescription(message.WeatherCondition)}");
        Console.WriteLine($"Mesajın Gönderildigi Saat: {DateTime.Now.ToString("HH:mm:ss")}");
        Console.WriteLine("------------------------------------");

        await Task.Delay(1000);  // 1 saniye gecikme
        currentDate = currentDate.AddDays(1);
    }

    Console.ReadLine();
}