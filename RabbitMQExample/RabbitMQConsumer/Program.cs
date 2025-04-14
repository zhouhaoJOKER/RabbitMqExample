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

// fanout 交换机模式下：队列名称需要选择随机名称，并且需要独占消息队列
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

const string exchangeName = "broker.direct";
const string queueName = "order.create"; // 队列名称
const string routeKey = queueName;

var queue = await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

await channel.QueueBindAsync(queue.QueueName, exchangeName, routeKey);

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
    autoAck: false,//关闭自动确认，手动确认，避免消息丢失，如果消息unack之后，链接关闭的时候，消息会重新入队
    consumer: consumer);

Console.WriteLine("RabbitMQ 消费者已启动，等待消息... (按Enter退出)");
Console.ReadLine();