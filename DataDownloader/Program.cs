using System;
using System.IO;
using System.Threading.Tasks;
using DataDownloader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataDownloader
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            
            await CreateHostBuilder(args).RunConsoleAsync();

            Console.WriteLine("Goodbye World!");
        }

        private static IConfiguration CreateConfiguration(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            return configuration;
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = CreateConfiguration(args);
            var urlRegex = new UrlRegex();
            configuration.GetSection("UrlRegex").Bind(urlRegex);

            var targetFolderSettings = new TargetFolderSettings();

            var ioReader = new IoReader(
                () => (char)Console.Read(),
                () => Console.In.Peek() == -1,
                () => Console.ReadLine(),
                () => (char)Console.In.Peek()
                );

            var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ConsoleHostedService>();
                services.AddSingleton(urlRegex);
                services.AddSingleton(targetFolderSettings);
                services.AddScoped<ICommandParser, CommandParser>();
                
                services.AddScoped<ICommandExecutor, CommandExecutor>();
                services.AddScoped<ICommandSeeder, CommandSeeder>();
                services.AddHttpClient();
                services.AddSingleton<IIoReader>(ioReader);
                services.AddScoped<IUrlVerifier, UrlVerifier>();
            });

            return hostBuilder;
        }
    }
}
