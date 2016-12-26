using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace QuizSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseStartup<AppStart>()
                    .Build();
            host.Run();
        }
    }
}
