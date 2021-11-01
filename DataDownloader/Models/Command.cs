using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task ExecuteCommandAsync(IEnumerable<string> args);
    }
    abstract class Command : ICommand
    {
        public Command(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public string Name { get; }
        public string Description { get; }
        public abstract Task ExecuteCommandAsync(IEnumerable<string> args);
    }
}
