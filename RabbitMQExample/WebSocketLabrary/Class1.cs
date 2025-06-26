using System;
using WebSocketSharp;

namespace WebSocketLabrary
{
    public class Class1
    {
        /// <summary>
        /// 建立链接
        /// </summary>
        public void LineUp()
        {
            try
            {
                //1、创建websocket链接
                // 使用 wss:// 安全连接地址
                var _ws = new WebSocket($"wss://campus.fmaster.cn/api/ws/lineup?access_token=&clientKey=&TENANT-ID=1");

                // 如果使用的是自签名证书，关闭证书验证（⚠️正式环境不建议关闭）
                _ws.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true; // 始终接受证书（开发测试环境）
                };

                // 注册事件
                _ws.OnOpen += (sender, e) => Console.WriteLine("WebSocket 连接已建立.");
                _ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine("收到消息: " + e.Data);
                    
                };
                _ws.OnError += (sender, e) =>
                {
                    Console.WriteLine("发生错误: " + e.Message); 
                };
                _ws.OnClose += (sender, e) =>
                {
                    Console.WriteLine("连接已关闭: " + e.Reason + e.Code); 
                };
                _ws.Connect(); 
            }
            catch (Exception ex)
            {
                //TODO：记录日志
            }
        }
    }
}
