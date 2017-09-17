using System.Net;

namespace MyServer.Web
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) => 
            WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 81);
                    options.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.UseHttps("atanas.pfx", "naseto");
                    });
                })
            .Build();

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
    }
}