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
        Action<IEnumerable<string>> ExecuteCommand { get; set; }
    }
    class Command : ICommand
    {
        public Command(string name, Action<IEnumerable<string>> command)
        {
            Name = name;
            ExecuteCommand = command;
        }
        public string Name { get; }
        public Action<IEnumerable<string>> ExecuteCommand { get; set; }
    }
}
