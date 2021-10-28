using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataDownloader
{
    //taken from: https://dfederm.com/building-a-console-app-with-.net-generic-host/
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IIoReader _ioReader;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IIoReader ioReader)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _ioReader = ioReader;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Hello World!");
                        ReadCharDelegate read = () => (char)Console.Read();
                        CheckIfIoIsEmpty isEmpty = () => Console.In.Peek() == -1;
                        while (true)
                        {
                            var strings = _ioReader.ReadUntil(";", read, isEmpty);
                            foreach (var _string in strings)
                            {
                                Console.WriteLine("Next element: ");
                                Console.WriteLine(_string);
                            }
                            
                        }
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
