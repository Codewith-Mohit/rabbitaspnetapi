using System.Text;
using RabbitMQ.Client;

class Program
{
    static async Task Main()
    {
        // RabbitMQ server connection parameters
        var factory = new ConnectionFactory
        {
            HostName = "132.220.74.223",    // Replace with your RabbitMQ server's hostname
            Port = 5672,               // Default RabbitMQ port            
            UserName = "guest",        // RabbitMQ username
            Password = "guest",       // RabbitMQ password
            ClientProvidedName = "Rabbit Sender"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        //Exchange-name
        string exchangeName = "DemoExchange";
        //Routing Key
        string routingKey = "demoRoutingKey";
        //Declare a queue
        string queueName = "DemoQueue";  // Replace with your queue name

        await channel.ExchangeDeclareAsync(exchangeName, type: ExchangeType.Direct, durable: false, autoDelete: false, arguments: null, passive: false, noWait: default);
        await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queueName, exchangeName, routingKey, arguments: null);

        for (int i = 0; i < 60; i++)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello, RabbitMQ! Here is message #{i}");
            // Publish the message to the queue
            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, body: messageBodyBytes);
            Thread.Sleep(1000);

            Console.WriteLine($"Message Sent: {Encoding.UTF8.GetString(messageBodyBytes)}");
        }
        Console.ReadLine();
    }
}