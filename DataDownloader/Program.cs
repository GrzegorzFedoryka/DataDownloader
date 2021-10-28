﻿using System;
using System.IO;
using System.Threading.Tasks;
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

            var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ConsoleHostedService>();
                services.AddSingleton(urlRegex);
                services.AddScoped<IIoReader, IoReader>();
                services.AddScoped<ICommandParser, CommandParser>();
            });

            return hostBuilder;
        }
    }
}
