﻿using RabbitMQ.Client;
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

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

const string exchangeName = "broker.topic";
const string queueName = "order.create.new"; // 队列名称
const string routeKey = queueName;

await channel.ExchangeDeclareAsync(exchange: exchangeName, ExchangeType.Topic, true, false);
await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
await channel.QueueBindAsync(queueName, exchangeName, routeKey);

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

    var props = new BasicProperties();
    props.ContentType = "text/plain";
    props.DeliveryMode = DeliveryModes.Persistent;

    await channel.BasicPublishAsync(exchangeName, routingKey: routeKey, mandatory: false, basicProperties: props, body: body);

    Console.WriteLine($"生产者 [x] routeKey:{routeKey} 已发送: {message.Content}");
}

Console.WriteLine("生产者已停止");