using DataDownloader.Commands;
using DataDownloader.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    interface ICommandExecutor
    {
        Task ExecuteCommandAsync(IEnumerable<string> args);
    }
    class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandParser _parser;
        private readonly ILogger<CommandExecutor> _logger;
        private readonly ICommandSeeder _seeder;
        private ICommand[] Commands { get; }

        public CommandExecutor(ICommandParser parser, ILogger<CommandExecutor> logger, ICommandSeeder seeder)
        {
            _parser = parser;
            _logger = logger;
            _seeder = seeder;

            Commands = _seeder.GetCommands();
        }
        public async Task ExecuteCommandAsync(IEnumerable<string> args)
        {
            var command = Commands.FirstOrDefault(c => args.Contains(c.Name));
            if(command == null)
            {
                _logger.LogInformation("Command doesn't exist. Try help to check available commands list.");
                return;
            }
            _logger.LogInformation($"Command {command.Name} is being executed.");
            await command.ExecuteCommandAsync(args.Skip(1));
        }
        private object HelpCommand(IEnumerable<string> args)
        {
            foreach (var command in Commands)
            {
                Console.WriteLine(command.Description);
            }
            return null;
        }

    }
}
