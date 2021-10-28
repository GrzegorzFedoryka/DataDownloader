using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace DataDownloader
{
    public delegate char ReadCharDelegate();
    public delegate bool CheckIfIoIsEmpty();
    interface IIoReader
    {
        IEnumerable<string> ReadUntil(string delimiter, ReadCharDelegate read, CheckIfIoIsEmpty isEmpty);
    }
    //taken from https://stackoverflow.com/questions/6655246/how-to-read-text-file-by-particular-line-separator-character
    class IoReader : IIoReader
    {
        public IEnumerable<string> ReadUntil(string delimiter, ReadCharDelegate read, CheckIfIoIsEmpty isEmpty)
        {
            var buffer = new List<char>();
            var delim_buffer = new CircularBuffer<char>(delimiter.Length);
            char c = '\0';
            bool isReadingEnded = false;
            while (true)
            {
                if (isReadingEnded)
                    yield break;
                try
                {
                    if (!isEmpty())
                    {
                        c = read();
                    }
                    else
                    {
                        isReadingEnded = true;
                    }
                    
                }
                catch (Exception)
                {
                    Console.WriteLine("Unhandled exception");
                }

                if (isReadingEnded)
                {
                    if(delim_buffer.ToString() == delimiter)
                    {
                        Console.WriteLine("to już koniec");
                        yield return new String(buffer.ToArray()).Substring(0, buffer.Count - delimiter.Length);
                    }
                    else
                    {
                        Console.WriteLine("to już koniec2");
                        yield return new String(buffer.ToArray());
                    }
                }

                delim_buffer.Enqueue(c);
                buffer.Add(c);
                if (delim_buffer.ToString() == delimiter)
                {
                    Console.WriteLine("hej to ja, kolejny element");
                    yield return new String(buffer.ToArray()).Substring(0, buffer.Count - delimiter.Length);
                    buffer.Clear();
                }
            }
        }
        private class CircularBuffer<T> : Queue<T>
        {
            private int _capacity;

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
                List<String> items = new List<string>();
                foreach (var x in this)
                {
                    items.Add(x.ToString());
                };
                return String.Join("", items);
            }
        }
    }
}
