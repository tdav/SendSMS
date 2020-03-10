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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()

            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseIISIntegration();
                // webBuilder.UseWebRoot(@"c:\Works\Prep.uz\SmsSender\ConsoleApp1\SmsGatePublish");
                // webBuilder.UseUrls("https://*:48081", "http://*:48080");
                webBuilder.UseStartup<Startup>();
            });
    }
}
