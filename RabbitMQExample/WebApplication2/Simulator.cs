namespace WebApplication2
{
    public class Simulator
    {
        public ZteApi zteApi = new ZteApi();
        public Simulator()
        {
            
        }

        public void InitToken(string _token) 
        {
            zteApi.InitToken(_token);
        }

        public string GetToken() => zteApi.token;
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
