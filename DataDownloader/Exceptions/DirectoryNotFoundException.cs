using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Exceptions
{
    class DirectoryNotFoundException : Exception
    {
        public DirectoryNotFoundException(string message) : base(message)
        {

        }
    }
}
