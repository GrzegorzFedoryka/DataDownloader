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
        private readonly IIoReader _reader;

        public CommandParser(IIoReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<string> GetArguments(Func<char> read, Func<bool> isEmpty, Func<char> peek)
        {
            //io generator
            var readings = _reader.ReadUntil(" ", read, isEmpty);
            var commandArguments = new List<string>();

            var commandName = readings.First();
            commandArguments.Add(commandName);

            while(_reader.PeekIo(peek) == '-')
            {
                commandArguments.Add(readings.First());
            }

            return commandArguments;

        }
    }
}
