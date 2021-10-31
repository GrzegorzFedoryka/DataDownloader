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
        object ExecuteCommand(IEnumerable<string> args);
    }
    class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandParser _parser;
        private readonly ILogger<CommandExecutor> _logger;
        private readonly ICommand[] commands =
        {
            new Command("read", (IEnumerable<string> t) => t.First(), "xD"),
            new Command("read2", (IEnumerable<string> t) => {
                Console.WriteLine("hehe");
                return null;
            }, "xD"),
            new Command("targetfolder", 
                (args) => {
                    var settings = new TargetFolderSettings { IsToCreateDirectiory = args.Contains("-c"), IsRelative = args.Contains("-r") };
                    var directory = IoReader.ReadToEnd(
                        () => Console.ReadLine(),
                        () => Console.In.Peek() == -1);
                    settings.Directory = string.IsNullOrEmpty(directory) ? AppDomain.CurrentDomain.BaseDirectory : directory;
                    return settings;
                }, 
                "  ") //todo
        };

        public CommandExecutor(ICommandParser parser, ILogger<CommandExecutor> logger)
        {
            _parser = parser;
            _logger = logger;
        }
        public object ExecuteCommand(IEnumerable<string> args)
        {
            var command = commands.FirstOrDefault(c => args.Contains(c.Name));
            if(command == null)
            {
                _logger.LogInformation("Command doesn't exist. Try help to check available commands list.");
                return null;
            }
            _logger.LogInformation($"Command {command.Name} is being executed.");
            var result = command.ExecuteCommand(args.Skip(1));
            return result;
        }
        private object HelpCommand(IEnumerable<string> args)
        {
            foreach (var command in commands)
            {
                Console.WriteLine(command.Description);
            }
            return null;
        }

    }
}
