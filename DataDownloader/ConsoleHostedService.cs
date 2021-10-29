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
        private readonly ICommandParser _commandParser;
        private readonly IUrlVerifier _urlVerifier;
        private readonly ICommandExecutor _commandExecutor;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IIoReader ioReader,
            ICommandParser commandParser,
            IUrlVerifier urlVerifier,
            ICommandExecutor commandExecutor)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _ioReader = ioReader;
            _commandParser = commandParser;
            _urlVerifier = urlVerifier;
            _commandExecutor = commandExecutor;
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

                        var commandArguments = _commandParser.GetArguments(
                            () => (char)Console.Read(),
                            () => Console.In.Peek() == -1,
                            () => (char)Console.In.Peek());
                        //foreach(var arg in commandArguments)
                        //{
                        //    Console.WriteLine(arg);
                        //}
                        _commandExecutor.ExecuteCommand(commandArguments);
                        var strings = _ioReader.ReadUntil(          //generator which reads from io stream
                                ";",                                // delimiter
                                () => (char)Console.Read(),         // stream read single char delegate
                                () => Console.In.Peek() == -1);     // peek in input stream delegate

                        foreach (var _string in strings)
                        {
                            var isCorrect = _urlVerifier.IsUrlCorrect(_string);
                            var exists = _urlVerifier.UrlExists(_string);
                            Console.WriteLine("Next element: ");
                            Console.WriteLine($"{_string} is correct: {isCorrect} exists: {exists}");
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
