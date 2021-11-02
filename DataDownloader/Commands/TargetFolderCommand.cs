using DataDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Commands
{
    public class TargetFolderCommand : Command, ICommand
    {
        private readonly TargetFolderSettings _settings;
        private readonly IIoReader _reader;

        public TargetFolderCommand(string name, 
            string description, 
            TargetFolderSettings settings,
            IIoReader reader) : base(name, description)
        {
            _settings = settings;
            _reader = reader;
        }
        public override async Task ExecuteCommandAsync(IEnumerable<string> args)
        {
            await Task.Run(() =>
            {
                _settings.IsToCreateDirectiory = args.Contains("-c");
                _settings.IsRelative = args.Contains("-r");
                var directory = _reader.ReadToEnd();
                _settings.Directory = string.IsNullOrEmpty(directory) ? AppDomain.CurrentDomain.BaseDirectory : directory;
                Console.WriteLine($"Target folder is set: {_settings.Directory}");
            });
        }
    }
}
