using ConsoleApp1.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSocketSharp;

namespace ConsoleApp1
{
    public class CampusProxy : ICampusProxy
    {
        private readonly string BaseUrl = "https://campus.fmaster.cn";
        private readonly string BaseWsHost = "wss://campus.fmaster.cn";
        private readonly string ClientKey = "4gwy";
        private string _accssToken;
        private RestClient _client;
        private WebSocket _ws;
        private bool _isLineUp;

        private Timer _heartbeatTimer;

        public CampusProxy()
        {
            _client = new RestClient(new Uri(BaseUrl));
            _client.AddDefaultHeader("clientKey", ClientKey);
        }

        /// <summary>
        /// 使用webUrl启动
        /// </summary>
        /// <param name="accssToken"></param>
        public CampusProxy(string accssToken)
        {
            _accssToken = accssToken;
            _client = new RestClient(new Uri(BaseUrl));
            _client.AddDefaultHeader("clientKey", "4gwy");
        }

        /// <summary>
        /// 建立链接
        /// </summary>
        public void LineUp()
        {
            try
            {
                if (string.IsNullOrEmpty(_accssToken))
                {
                    throw new ArgumentNullException("access_token");
                }

                if (_isLineUp)
                {
                    return;
                }

                //1、创建websocket链接
                // 使用 wss:// 安全连接地址
                _ws = new WebSocket($"{BaseWsHost}/api/ws/lineup?access_token={_accssToken}&clientKey={ClientKey}&TENANT-ID=1");

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
                    var res = JsonConvert.DeserializeObject<ApiResponse<BaseMesModel>>(e.Data);

                    if (res != null && res.Code == 0)
                    {
                        if (res.Data.type == 3)
                        {
                            _isLineUp = true;
                            //Console.WriteLine("lineUp");
                        }

                        if (res.Data.type == 4)
                        {
                            ////TODO:
                            //Console.WriteLine("开始学习");
                        }
                    }
                };
                _ws.OnError += (sender, e) =>
                {
                    Console.WriteLine("发生错误: " + e.Message);
                    if (_isLineUp) _isLineUp = false;
                };
                _ws.OnClose += (sender, e) =>
                {
                    Console.WriteLine("连接已关闭: " + e.Reason + e.Code);
                    if (_isLineUp) _isLineUp = false;
                };
                _ws.Connect();

                _heartbeatTimer = new Timer(_ => HeartBeat(), null, 3 * 1000, 5 * 1000);
            }
            catch (Exception ex)
            {
                //TODO：记录日志
            }
        }

        /// <summary>
        /// 开始实验
        /// </summary>
        /// <param name="taskId"></param>
        public void StartTask(long taskId)
        {
            try
            {
                if (_ws == null || _ws.ReadyState != WebSocketState.Open || _isLineUp == false)
                {
                    throw new ArgumentNullException("ws");
                }

                StartTaskModel startTaskModel = new StartTaskModel()
                {
                    taskId = taskId,
                };

                string data = JsonConvert.SerializeObject(startTaskModel);
                Console.WriteLine($"开始学习:taskId>{taskId}");
                _ws.Send(data);
            }
            catch (Exception ex)
            {
                //记录日志
                Console.WriteLine($"发送课程实验消息失败" + ex.Message);
            }
        }

        /// <summary>
        /// 结束实验
        /// </summary>
        public void EndTask()
        {
            try
            {
                if (_ws == null || _ws.ReadyState != WebSocketState.Open || _isLineUp == false)
                {
                    throw new ArgumentNullException("ws");
                }

                EndTaskModel endTaskModel = new EndTaskModel();

                string data = JsonConvert.SerializeObject(endTaskModel);
                _ws.Send(data);
            }
            catch (Exception ex)
            {
                //记录日志
            }
        }

        public void HeartBeat()
        {
            try
            {
                if (_ws == null || _ws.ReadyState != WebSocketState.Open || _isLineUp == false)
                {
                    throw new ArgumentNullException("ws");
                }

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} >> HeartBeat");
                HeartBeatModel model = new HeartBeatModel();
                string data = JsonConvert.SerializeObject(model);
                _ws.Send(data);

            }
            catch (Exception ex)
            {
                //记录日志
                Console.WriteLine($"发送心跳失败" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Login(string userName, string password)
        {
            RestRequest restRequest = new RestRequest("/api/client/v1/login", Method.Post);
            restRequest.AddJsonBody(new { userName, password = AesCFB128Encryptor.Encrypt(password, "fmasterultralabx") });
            var res = _client.Execute<ApiResponse<LoginResModel>>(restRequest);
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //请求失败,记录日志
                return;
            }

            if (res.Data != null && res.Data.Data != null)
            {
                this._accssToken = res.Data.Data.AccessToken;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdLine"></param>
        /// <returns></returns>
        public CampusCmdParam? DecodeCmdParam(string cmdLine)
        {
            string secret = HttpUtility.UrlDecode(cmdLine);
            // 3. base64解码
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(secret));

            // 4. 反序列化为 CampusCmdParam
            return JsonConvert.DeserializeObject<CampusCmdParam>(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string CreateWebUrl()
        {
            CampusCmdParam campusCmdParam = new CampusCmdParam()
            {
                token = "a8ae84fd-bd28-405c-9dcc-d93be4dda3f8",
                expId = "1935148883028127746"
            };
            string str = JsonConvert.SerializeObject(campusCmdParam);
            var data = Encoding.UTF8.GetBytes(str);
            var byets = Convert.ToBase64String(data);
            var s = HttpUtility.UrlEncode(byets);

            return $"MNOS://{s}";
        }

        public void Dispose()
        {
            this._ws.Close();
            this._isLineUp = false;
            this._accssToken = "";
            this._heartbeatTimer.Dispose();
            this._client.Dispose();
        }
    }
}
