namespace ConsoleApp1.Model
{
    public class LoginResModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }


        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }
        public string NickName { get; set; }
        ///...忽略其他的字段
    }

    public class ApiResponse<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
