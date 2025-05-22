using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// WebSocket 链接
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")] 
    public class WebSocketsController : ControllerBase
    {
        private readonly ILogger<WebSocketsController> _logger;
        private readonly UserSessionContext _userSessionContext;

        public WebSocketsController(
            ILogger<WebSocketsController> logger, UserSessionContext userSessionContext)
        {
            this._logger = logger;
            _userSessionContext = userSessionContext;
        }

        /// <summary>
        /// 1、用户需要登录fm之后，才能进行websocket链接
        /// 2、当等待绑定的用户分配到了zte账号之后，主动返回客户端并且更新新的token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task ws()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? "";
                token = token.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    HttpContext.Response.StatusCode = 401; // Unauthorized
                    return;
                }
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                _userSessionContext.AddWebSocket(token, webSocket);
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400; // Bad Request
            }
        }

        private async Task Echo(WebSocket ws)
        {
            try
            {
                var buffer = new byte[1024 * 32];
                //当前这行代码会阻塞，直到有数据到达
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                string lastMessage = "";
                while (!result.CloseStatus.HasValue)
                {
                    lastMessage += Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (result.EndOfMessage)
                    {
                        try
                        {
                            await MsgHandler(lastMessage, ws);
                        }
                        catch (Exception ex)
                        {//
                        }
                        lastMessage = "";
                    }

                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private async Task MsgHandler( string message, WebSocket ws)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var msgs = message.Split("<@#row>", StringSplitOptions.RemoveEmptyEntries);
            _logger.LogInformation($"msgs:\n{msgs}");
            foreach (var msg in msgs)
            {
                try
                { 
                    switch (msg.ToLower())
                    {
                        case "init":
                            {
                                 
                            }
                            break; 
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
