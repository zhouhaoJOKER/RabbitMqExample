using ZteCommom.Book;

namespace WebApplication2
{
    public class Simulator
    {
        public ZteApi zteApi = new ZteApi();
        private readonly BookManager _bookManager;
        public Simulator(BookManager bookManager)
        {
            _bookManager = bookManager;
        }

        public void InitToken(string _token)
        {
            zteApi.InitToken(_token);
        }

        public string GetToken() => zteApi.token;

        public async Task Yuyue(int devNum, string token)
        {
            if (_bookManager.yuyueInfos.Count == 0 || _bookManager.yuyueInfos.First().token == token)
            {
                var res = Require(devNum);
                if (res)
                {
                }
                else
                {
                    int waitTokens = _bookManager.Push(token, devNum, YuyueSucess);
                }
            }
            else
            {
                int waitTokens = _bookManager.Push(token, devNum, YuyueSucess);
            }
        }

        public bool Require(int devNum)
        {
            return devNum <= 2;
        }

        public void YuyueSucess(int reqNum, string resourceId)
        {
            Console.WriteLine(" YuyueSucess:" + resourceId);
        }
    }

    public class ZteApi
    {
        public string token { get; set; }
        public ZteApi() { }

        public void InitToken(string _token)
        {
            token = _token;
        }
    }
}
