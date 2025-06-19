using RestSharp;
namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string userName = "zhouhao";
            string password = "123456";

            ICampusProxy proxy;
            string token = "";
            if (args.Length > 0)
            {
                token = args[0];
                Console.WriteLine($"token>>{token}");
                proxy = new CampusProxy(token);
            }
            else
            {
                proxy = new CampusProxy();
                proxy.Login(userName, password);
            }

            proxy.LineUp();

            List<long> taskList = new List<long>()
            {
                1935148883028127746,
                1935149032806723586,
                1935149108149006338,
                1935149242157019138,
                1935149347580850177,
            };

            await Task.Delay(1000);

            foreach (var item in taskList)
            {
                proxy.StartTask(item);
                await Task.Delay(5000);
                proxy.EndTask();
                await Task.Delay(5000);
            }


            await Task.Delay(3000);
            proxy.Dispose();


            Console.ReadKey();
        }
    }
}
