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
        Func<IEnumerable<string>, object> ExecuteCommand { get; set; } //
    }
    class Command : ICommand
    {
        public Command(string name, Func<IEnumerable<string>, object> command, string description)
        {
            Name = name;
            Description = description;
            ExecuteCommand = command;
        }
        public string Name { get; }
        public string Description { get; }
        public Func<IEnumerable<string>, object> ExecuteCommand { get; set; }
    }
}
