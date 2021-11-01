using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Exceptions
{
    public class DelimitersOverlapException : Exception
    {
        public DelimitersOverlapException(string message) : base(message)
        {

        }
    }
}
