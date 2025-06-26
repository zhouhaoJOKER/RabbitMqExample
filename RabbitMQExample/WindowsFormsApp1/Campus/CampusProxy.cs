using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebSocketSharp.Net.WebSockets;
using System.Threading;
using FM.UltraLab.Api.Model.CampusModel;
using WebSocketSharp;

namespace FM.UltraLab.Api.Campus
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
                _ws.SslConfiguration.EnabledSslProtocols =  System.Security.Authentication.SslProtocols.Tls12;
                //_ws.SslConfiguration.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
                
                // 注册事件
                _ws.OnOpen += (sender, e) => Console.WriteLine("WebSocket 连接已建立.");
                _ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine("收到消息: " + e.Data);
                    if (string.IsNullOrWhiteSpace(e.Data)) return;

                    //CamApiResponse<BaseMesModel> res = e.Data.ToObj<CamApiResponse<BaseMesModel>>();

                    //if (res != null && res.Code == 0)
                    //{
                    //    if (res.Data.type == 3)
                    //    {
                    //        _isLineUp = true;
                    //        //Console.WriteLine("lineUp");
                    //    }

                    //    if (res.Data.type == 4)
                    //    {
                    //        ////TODO:
                    //        //Console.WriteLine("开始学习");
                    //    }
                    //}
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
                 
            }
            catch (Exception ex)
            {
                //TODO：记录日志
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Login(string userName, string password)
        {
            RestRequest restRequest = new RestRequest("/api/client/v1/login", Method.POST);
            restRequest.AddJsonBody(new { userName, password = AesCFB128Encryptor.Encrypt(password, "fmasterultralabx") });
            var res = _client.Execute<CamApiResponse<LoginResModel>>(restRequest);
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
    }
}
