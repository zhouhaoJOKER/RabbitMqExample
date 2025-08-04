using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using WatsonTcp;

namespace WatsonTcpClientDemo
{
    class Program
    {
        private static WatsonTcpClient _client;
        static async Task Main()
        {
            _client = new WatsonTcpClient("127.0.0.1", 8075);

            _client.Events.ServerConnected += (s, e) =>
            {
                Console.WriteLine("Connected to server.");
            };

            _client.Events.ServerDisconnected += (s, e) =>
            {
                Console.WriteLine("Disconnected from server.");
            };

            _client.Events.MessageReceived += (s, e) =>
            {
                string response = Encoding.UTF8.GetString(e.Data);
                Console.WriteLine($"Server replied: {response}");
            };

            try
            {
                _client.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"链接失败{ex.StackTrace}{ex.Message}");
                return;
            }

            await MockSceneOne();
            await MockSceneTwo();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// 模拟场景1
        /// 1、设计一个电路并且使用两个电阻测量其中一个电阻的两端电压
        /// </summary>
        static async Task MockSceneOne()
        {
            if (_client.Connected)
            {
                Dictionary<string, object> metaData = new Dictionary<string, object>();
                metaData.Add("OptType", "LoadCir");
                string payload = @"{""CirId"":""1"",""DebugLevel"":""1"",""Cmds"":""[\""DESC\"",\""V1 pt1 0 SINE(0 0.502 1022)\"",\""R1 pt1 0 90G\"",\"".TRAN 1E-06S 0.0500000007450581S\"",\"".control\"",\""run\"",\"".endc\"",\"".end\""]"",""Speaker"":""{}"",""MonitorDevices"":""{\""示波器1\"":{\""CH1节点\"":\""pt1,0\"",\""CH2节点\"":\"",\"",\""DeviceType\"":\""示波器\""}}"",""StepTime"":""1E-06"",""CirMode"":""0"",""Tps"":""[\""pt1\"",\""0\""]""}";

                try
                {
                    var res = await _client.SendAndWaitAsync(10 * 1000, payload, metaData);
                    string data = Encoding.UTF8.GetString(res.Data);
                    Console.WriteLine($"MockSceneOne:\n{data}");
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"操作超时：LoadCir");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"其他错误：{ex.StackTrace}{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 模拟场景二
        /// 2、设计一个示波器的例子
        /// </summary>
        /// <returns></returns>
        static async Task MockSceneTwo()
        {
            if (!_client.Connected)
            {
                return;
            }

            
            string data = "";
            try
            {
                Dictionary<string, object> metaData = new Dictionary<string, object>();
                metaData.Add("OptType", "SBQSetting");

                string payload = @"{""水平展宽"":""0.002"",""触发通道"":""0"",""触发电平"":""0.000"",""触发类型"":""0"",""水平位置"":""0.000000"",""CH1显示模式"":""0"",""CH2显示模式"":""0"",""Name"":""示波器1""}";
                var res = await _client.SendAndWaitAsync(10 * 1000, payload, metaData);
                data = Encoding.UTF8.GetString(res.Data);
                Console.WriteLine($"MockSceneOne:\n{data}");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"操作超时：LoadCir");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"其他错误：{ex.StackTrace}{ex.Message}");
            }

            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            MessageResp? resp = JsonConvert.DeserializeObject<MessageResp>(data);

            if (resp == null || !resp.bSucess) 
            {
                return;
            }

            //LoadCir
            //try
            //{
            //    Dictionary<string, object> metaData = new Dictionary<string, object>();
            //    metaData.Add("OptType", "LoadCir");

            //    string payload = @"{""CirId"":""73"",""DebugLevel"":""0"",""Cmds"":""[\""DESC\"",\""V1 pt1 0 SINE(0 0.502 1022)\"",\""R1 pt1 0 90G\"",\"".TRAN 1E-06S 0.0520000006072223S\"",\"".control\"",\""run\"",\"".endc\"",\"".end\""]"",""Speaker"":""{}"",""MonitorDevices"":""{\""示波器1\"":{\""CH1节点\"":\""pt1,0\"",\""CH2节点\"":\"",\"",\""DeviceType\"":\""示波器\""}}"",""StepTime"":""1E-06"",""CirMode"":""0"",""Tps"":""[\""pt1\"",\""0\""]""}";
            //    var res = await _client.SendAndWaitAsync(10 * 1000, payload, metaData);
            //    data = Encoding.UTF8.GetString(res.Data);
            //    Console.WriteLine($"LoadCir:\n{data}");
            //}
            //catch (TimeoutException ex)
            //{
            //    Console.WriteLine($"操作超时：LoadCir");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"其他错误：{ex.StackTrace}{ex.Message}");
            //}

            //GetSBQData
            try
            {
                Dictionary<string, object> metaData = new Dictionary<string, object>();
                metaData.Add("OptType", "GetSBQData");

                string payload = @"{""水平展宽"":""0.002"",""触发通道"":""0"",""触发电平"":""0.000"",""触发类型"":""0"",""水平位置"":""0.000000"",""CH1显示模式"":""0"",""CH2显示模式"":""0"",""Name"":""示波器1"",""起始时间"":""0.0500000007450581""}";
                var res = await _client.SendAndWaitAsync(10 * 1000, payload, metaData);
                data = Encoding.UTF8.GetString(res.Data);
                Console.WriteLine($"GetSBQData:\n{data}");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"操作超时：GetSBQData");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"其他错误：{ex.StackTrace}{ex.Message}");
            }
        }

        /// <summary>
        /// 模拟场景三
        /// 3、设计一个定时需要上报数据的
        /// </summary>
        /// <returns></returns>
        static async Task MockSceneThird()
        {
            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("OptType", "HeartTopoGragh");
            string payload = "";
            var res = await _client.SendAsync(payload, metaData); 
        }
    }
}
