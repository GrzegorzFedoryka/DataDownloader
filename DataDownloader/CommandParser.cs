using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public interface ICommandParser
    {
        IEnumerable<string> GetArguments();
    }
    public class CommandParser : ICommandParser
    {
        private readonly IIoReader _reader;

        public CommandParser(IIoReader reader)
        {
            _reader = reader;
        }
        public IEnumerable<string> GetArguments()
        {
            var readings = _reader.ReadUntil(" ");
            var commandArguments = new List<string>();
            var enumerator = readings.GetEnumerator();
            enumerator.MoveNext();
            var commandName = enumerator.Current;
            if (!string.IsNullOrEmpty(commandName))
            {
                commandArguments.Add(commandName);
            }
            

            while(_reader.PeekIo() == '-')
            {
                enumerator.MoveNext();
                var argument = enumerator.Current;
                commandArguments.Add(argument);
            }

            return commandArguments;

        }
    }
}
