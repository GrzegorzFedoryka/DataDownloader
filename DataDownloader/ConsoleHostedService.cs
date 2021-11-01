using DataDownloader.Models;
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
        //private readonly IIoReader _ioReader;
        private readonly ICommandParser _commandParser;
        private readonly IUrlVerifier _urlVerifier;
        private readonly ICommandExecutor _commandExecutor;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            //IIoReader ioReader,
            ICommandParser commandParser,
            IUrlVerifier urlVerifier,
            ICommandExecutor commandExecutor)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            //_ioReader = ioReader;
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
                        var targetFolderSettings = new TargetFolderSettings();
                        Thread.Sleep(1000);
                        _logger.LogInformation("Hello World!");
                        while (true)
                        {
                            if(Console.In.Peek() == -1)
                            {
                                Console.WriteLine("Stream is empty");
                            }
                            else
                            {
                                Console.WriteLine("Console is not empty");
                            }
                            var commandNameWithFlags = _commandParser.GetArguments(
                                () => (char)Console.Read(),
                                () => Console.In.Peek() == -1,
                                () => (char)Console.In.Peek());

                            
                            if(commandNameWithFlags != null && commandNameWithFlags.Any())
                            {
                                Console.WriteLine(commandNameWithFlags.ToList()[0]);
                                await _commandExecutor.ExecuteCommandAsync(commandNameWithFlags);
                            }
                            
                        }
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
