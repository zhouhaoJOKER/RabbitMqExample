using RabbitMQ.Client;
using SharedModels;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory()
{
    HostName = "192.168.0.226", // RabbitMQ服务器地址
    Port = 5672,           // 默认端口
    UserName = "fzwebapi",    // 默认用户名
    Password = "123456",     // 默认密码
    VirtualHost = "fzwebapi",
};

using var connection = await factory.CreateConnectionAsync("Producer Node1");
using var channel = await connection.CreateChannelAsync();
//1、定义ExchangeType类型是 Fanout模式生产者
//2、注意这种方式的情况下，如果消费者还没有上线的话，消息会被丢失
const string exchangeName = "broker.fanout";
await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
Console.WriteLine("RabbitMQ 生产者已启动，输入消息内容并按Enter发送 (输入exit退出):");

while (true)
{
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    // 创建消息
    var message = new Message
    {
        Content = input,
        Timestamp = DateTime.UtcNow
    };

    // 序列化消息
    var json = JsonSerializer.Serialize(message);
    var body = Encoding.UTF8.GetBytes(json);

    //保证消息的持久化
    var props = new BasicProperties()
    {
        ContentType = "text/plain",
        DeliveryMode = DeliveryModes.Persistent
    };

    await channel.BasicPublishAsync(exchangeName, routingKey: "", mandatory: true, basicProperties: props, body: body);

    Console.WriteLine($" [x] 已发送: {message.Content}");
}

Console.WriteLine("生产者已停止");