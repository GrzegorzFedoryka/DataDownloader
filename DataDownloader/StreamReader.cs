using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace DataDownloader
{
    class StreamReader
    {
        public void RedirectToStreamReader(IHost host)
        {
            while(true){
            }
        }

        public static IEnumerable<string> ConsoleReadUntil(string delimiter)
        {
            var buffer = new List<char>();
            var delim_buffer = new CircularBuffer<char>(delimiter.Length);
            char c = '\0';
            bool isReadingEnded = false;
            while (true)
            {
                try
                {
                    c = (char)Console.Read();
                }
                catch (IOException)
                {
                    isReadingEnded = true; 
                }

                if (isReadingEnded)
                {
                    yield return new String(buffer.ToArray());
                    yield break;
                }

                delim_buffer.Enqueue(c);
                buffer.Add(c);
                if (delim_buffer.ToString() == delimiter)
                {
                    yield return new String(buffer.ToArray()).Substring(0, buffer.Count - delimiter.Length);
                    buffer.Clear();
                }

                    
            }
        }
        //Circular buffer taken from https://stackoverflow.com/questions/6655246/how-to-read-text-file-by-particular-line-separator-character
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
