using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Threading;
using DataDownloader.Exceptions;

namespace DataDownloader
{
    public interface IIoReader
    {
        char PeekIo();
        string ReadToEnd();
        IEnumerable<string> ReadUntil(string delimiter);
    }

    //taken from https://stackoverflow.com/questions/6655246/how-to-read-text-file-by-particular-line-separator-character
    public class IoReader : IIoReader
    {
        private readonly Func<char> _read;
        private readonly Func<bool> _isEmpty;
        private readonly Func<string> _readToEnd;
        private readonly Func<char> _checkNextChar;

        public IoReader(Func<char> read, Func<bool> isEmpty, Func<string> readToEnd, Func<char> checkNextChar)
        {
            _read = read;
            _isEmpty = isEmpty;
            _readToEnd = readToEnd;
            _checkNextChar = checkNextChar;
        }
        public IEnumerable<string> ReadUntil(string delimiter)
        {
            var buffer = new List<char>();
            var delim_buffer = new CircularBuffer<char>(delimiter.Length);
            char c;
            bool hasStartedReading = false;
            while (true)
            {
                if (!_isEmpty() || hasStartedReading == false)
                {
                    c = _read();
                    hasStartedReading = true;
                }
                else
                {
                    var bufferString = new String(buffer.ToArray());
                    if (bufferString != Environment.NewLine && bufferString.Length != 0)
                        yield return bufferString.Trim();
                    break;
                }

                delim_buffer.Enqueue(c);
                buffer.Add(c);

                if (delim_buffer.ToString() == delimiter)
                {
                    if (buffer.Count - delimiter.Length < 0)
                    {
                        throw new DelimitersOverlapException("Delimiters overlap theirselves!");
                    }
                    yield return new String(buffer.ToArray()).Substring(0, buffer.Count - delimiter.Length);
                    buffer.Clear();
                }
            }
            yield break;
        }

        public string ReadToEnd()
        {
            var buffer = new StringBuilder();
            if (!_isEmpty())
            {
                buffer.Append(_readToEnd());
            }
            else
            {
                return string.Empty;
            }
            return buffer.ToString();
        }
        public char PeekIo()
        {
            var nextChar = _checkNextChar();
            return nextChar;
        }
        private class CircularBuffer<T> : Queue<T>
        {
            private readonly int _capacity;

            public CircularBuffer(int capacity)
                : base(capacity)
            {
                _capacity = capacity;
            }

            new public void Enqueue(T item)
            {
                if (base.Count == _capacity)
                {
                    base.Dequeue();
                }
                base.Enqueue(item);
            }

            public override string ToString()
            {
                var items = new List<string>();
                foreach (var x in this)
                {
                    items.Add(x.ToString());
                };
                return String.Join("", items);
            }
        }
    }
}
