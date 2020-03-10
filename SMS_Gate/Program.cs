using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace SMS_Gate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"c:\SmsSender");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenAnyIP(48081, listenOptions =>
                        {
                            listenOptions.UseHttps(@"c:\SmsSender\makecert.pfx", "1234");
                        });
                    });
                    webBuilder.UseWebRoot(@"c:\SmsSender");
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseIISIntegration();
                   // webBuilder.UseUrls("https://*:48081");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
