using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    interface ICommandParser
    {
        IEnumerable<string> GetArguments(Func<char> read, Func<bool> isEmpty, Func<char> peek);
    }
    class CommandParser : ICommandParser
    {
        public IEnumerable<string> GetArguments(Func<char> read, Func<bool> isEmpty, Func<char> peek)
        {
            var readings = IoReader.ReadUntil(" ", read, isEmpty);
            var commandArguments = new List<string>();

            var commandName = readings.First();
            commandArguments.Add(commandName);

            while(IoReader.PeekIo(peek) == '-')
            {
                commandArguments.Add(readings.First());
            }

            return commandArguments;

        }
    }
}
