using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace WebApplication2
{
    public class UserSessionContext
    {
        private static readonly object locker = new object();
        /// <summary>
        /// 
        /// </summary> 
        private readonly ConcurrentDictionary<string, Simulator> _userSimulatorContext;
        private readonly ConcurrentDictionary<string, WebSocket> _userWebSocketContext;

        public UserSessionContext()
        {
            _userSimulatorContext = new ConcurrentDictionary<string, Simulator>();
            _userWebSocketContext = new ConcurrentDictionary<string, WebSocket>();
        }

        /// <summary>
        /// 添加用户仿真器
        /// </summary>
        /// <param name="loginToken">用户登录令牌</param>
        /// <param name="simulator">用户仿真器实例</param>
        public void AddUserSimulator(string? loginToken, Simulator simulator)
        {
            lock (locker)
            {
                if (!_userSimulatorContext.ContainsKey(loginToken))
                {
                    _userSimulatorContext.TryAdd(loginToken, simulator);
                }
            }
        }
        /// <summary>
        /// 移除用户仿真器
        /// </summary>
        /// <param name="loginToken">用户登录令牌</param>
        public void RemoveUserSimulator(string loginToken)
        {
            if (_userSimulatorContext.TryRemove(loginToken, out Simulator simulator))
            {
            }
        }

        /// <summary>
        /// 获取用户仿真器
        /// </summary>
        /// <param name="loginToken">用户登录令牌</param>
        /// <returns>用户仿真器实例，如果不存在则返回 null</returns>
        public Simulator GetUserSimulator(string loginToken)
        {
            if (!_userSimulatorContext.TryGetValue(loginToken, out Simulator? simulator))
            {

            }

            return simulator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public void AddWebSocket(string loginToken, WebSocket webSocket)
        {
            //lock (locker)
            //{
            //    if (!_userWebSocketContext.ContainsKey(loginToken))
            //    {
            //        _userWebSocketContext.TryAdd(loginToken, webSocket);
            //    }
            //}
            //本来就是原子操作无需再添加locker，避免了线程阻塞

            if (_userWebSocketContext.ContainsKey(loginToken))
            {
                _userWebSocketContext.Remove(loginToken, out _);
            }

            _userWebSocketContext.TryAdd(loginToken, webSocket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public WebSocket? GetUserSocket(string userId)
        {
            if (!_userWebSocketContext.TryGetValue(userId, out WebSocket? webSocket))
            {

            }
            return webSocket;
        }
    }
}
