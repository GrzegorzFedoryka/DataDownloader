using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Exceptions
{
    class UrlIncorrectException : Exception
    {
        public UrlIncorrectException(string message) : base(message)
        {

        }
    }
}
