namespace WebApplication2.Dto
{
    public class WSMessage
    {
        public string route { get; set; }
        public string msgId { get; set; }
        public string msg { get; set; }
        public string submsg { get; set; }
        public Dictionary<string, string> paramMap { get; set; } = new Dictionary<string, string>();
    }
}
