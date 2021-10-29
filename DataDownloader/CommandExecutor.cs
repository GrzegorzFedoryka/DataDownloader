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
        string ExecuteCommand(IEnumerable<string> args);
    }
    class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandParser _parser;
        private readonly ILogger<CommandExecutor> _logger;
        private readonly ICommand[] commands =
        {
            new Command("read", (IEnumerable<string> t) => t.First()),
            new Command("read2", (IEnumerable<string> t) => { Console.WriteLine("hehe"); "xD"; } //TODO
        };

        public CommandExecutor(ICommandParser parser, ILogger<CommandExecutor> logger)
        {
            _parser = parser;
            _logger = logger;
        }
        public string ExecuteCommand(IEnumerable<string> args)
        {
            var command = commands.FirstOrDefault(c => args.Contains(c.Name));
            if(command == null)
            {
                _logger.LogInformation("Command doesn't exist. Try help to check available commands list.");
                return null;
            }
            _logger.LogInformation($"Command {command.Name} is being executed.");
            command.ExecuteCommand(args.Skip(1));
            return command.Name;
        }
    }
}
