// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


// RabbitMQ server connection parameters
var factory = new ConnectionFactory
{
    HostName = "localhost",    // Replace with your RabbitMQ server's hostname
    Port = 5672,               // Default RabbitMQ port
    UserName = "guest",        // RabbitMQ username
    Password = "guest",       // RabbitMQ password
    ClientProvidedName = "Rabbit Reciever"      
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

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (sender, args) =>
{
    var body = args.Body.ToArray();

    string msg = Encoding.UTF8.GetString(body);

    Console.WriteLine( $"Message Recieved : {msg}");

    channel.BasicAckAsync(args.DeliveryTag, multiple: false);
    //throw new NotImplementedException();
};