using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace DataDownloader
{

    //taken from https://stackoverflow.com/questions/6655246/how-to-read-text-file-by-particular-line-separator-character
    class IoReader
    {
        public static IEnumerable<string> ReadUntil(string delimiter, Func<char> read, Func<bool> isEmpty)
        {
            var buffer = new List<char>();
            var delim_buffer = new CircularBuffer<char>(delimiter.Length);
            char c;
            bool hasStartedReading = false;
            while (true)
            {
                if (!isEmpty() || hasStartedReading == false)
                {
                    c = read();
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
                        throw new IndexOutOfRangeException("Delimiters overlap theirselves!");
                    }
                    yield return new String(buffer.ToArray()).Substring(0, buffer.Count - delimiter.Length);
                    buffer.Clear();
                }
            }
            yield break;
        }

        public static string ReadToEnd(Func<string> readToEnd, Func<bool> isEmpty)
        {
            var buffer = new StringBuilder();
            if (!isEmpty())
            {
                buffer.Append(readToEnd());
            }
            else
            {
                return null;
            }
            return buffer.ToString();
        }
        public static char PeekIo(Func<char> checkNextChar)
        {
            var nextChar = checkNextChar();
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
