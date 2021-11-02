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
        private readonly ICommandParser _commandParser;
        private readonly ICommandExecutor _commandExecutor;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            ICommandParser commandParser,
            ICommandExecutor commandExecutor)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _commandParser = commandParser;
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
                            var commandNameWithFlags = _commandParser.GetArguments();

                            
                            if(commandNameWithFlags != null && commandNameWithFlags.Any())
                            {
                                _logger.LogInformation(commandNameWithFlags.ToList()[0]);
                                try
                                {
                                    await _commandExecutor.ExecuteCommandAsync(commandNameWithFlags);
                                }
                                catch
                                {
                                    _logger.LogError("Unable to execute command. Try execute command without flags.");
                                }
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
