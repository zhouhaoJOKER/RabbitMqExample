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

        public UserSessionContext()
        {
            _userSimulatorContext = new ConcurrentDictionary<string, Simulator>();
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
    }
}
