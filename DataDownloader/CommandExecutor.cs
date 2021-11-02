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
    public interface ICommandExecutor
    {
        Task ExecuteCommandAsync(IEnumerable<string> args);
    }
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ILogger<CommandExecutor> _logger;
        private readonly ICommandSeeder _seeder;
        private ICommand[] Commands { get; }

        public CommandExecutor(ILogger<CommandExecutor> logger, ICommandSeeder seeder)
        {
            _logger = logger;
            _seeder = seeder;

            Commands = _seeder.GetCommands();
        }
        public async Task ExecuteCommandAsync(IEnumerable<string> args)
        {
            if (args.Contains("help"))
            {
                HelpCommand();
                return;
            }
            var command = Commands.FirstOrDefault(c => args.Contains(c.Name));
            if(command == null)
            {
                _logger.LogInformation("Command doesn't exist. Try help to check available commands list.");
                return;
            }
            _logger.LogInformation($"Command {command.Name} is being executed.");
            await command.ExecuteCommandAsync(args.Skip(1));
        }
        private void HelpCommand()
        {
            foreach (var command in Commands)
            {
                Console.WriteLine($"{command.Name}: {command.Description}");
            }
        }

    }
}
