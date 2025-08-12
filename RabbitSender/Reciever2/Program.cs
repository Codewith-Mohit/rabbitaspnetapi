
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


// RabbitMQ server connection parameters
var factory = new ConnectionFactory
{
    HostName = "132.220.74.223",    // Replace with your RabbitMQ server's hostname
    Port = 5672,               // Default RabbitMQ port
    UserName = "guest",        // RabbitMQ username
    Password = "guest",       // RabbitMQ password
    ClientProvidedName = "Rabbit Reciever2 App"
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

Console.WriteLine("Waiting for msg..");

//await channel.BasicQosAsync( 2, 1, global: false,default);

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) => {

    Task.Delay(3000).Wait();

    var body = ea.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Recieved : {message}");

    channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

    return Task.CompletedTask;
};

string consumerTag = await channel.BasicConsumeAsync(queueName, false, consumer, default);

Console.ReadLine();

await channel.CloseAsync();

await connection.CloseAsync();