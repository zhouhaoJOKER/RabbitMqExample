using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels;
using System.Text;
using System.Text.Json;

// 创建连接工厂
var factory = new ConnectionFactory()
{
    HostName = "192.168.0.226", // RabbitMQ服务器地址
    Port = 5672,           // 默认端口
    UserName = "fzwebapi",    // 默认用户名
    Password = "123456",    // 默认密码
    VirtualHost = "fzwebapi"
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// 声明队列(确保队列存在)
//const string queueName = "messageQueue_no2";
const string exchangeName = "broker1";

await channel.ExchangeDeclareAsync(exchange: exchangeName, ExchangeType.Fanout, true, false);

//Direct
//await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
//fanout模式下，队列名称可以不一致
var queue = await channel.QueueDeclareAsync(queue: "", durable: true, exclusive: true, autoDelete: false, arguments: null);

await channel.QueueBindAsync(queue.QueueName, exchangeName, "");

// 创建消费者
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var message = JsonSerializer.Deserialize<Message>(json);

        Console.WriteLine($" [x] 收到消息: {message?.Content} (发送于: {message?.Timestamp:yyyy-MM-dd HH:mm:ss})");

        // 手动确认消息
        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [!] 处理消息时发生错误: {ex.Message}");
        // 如果需要，可以选择不确认消息或将其重新入队
        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
    }
};


// 开始消费消息
await channel.BasicConsumeAsync(
    queue: queue.QueueName,
    autoAck: false,
    consumer: consumer);

Console.WriteLine("RabbitMQ 消费者已启动，等待消息... (按Enter退出)");
Console.ReadLine();