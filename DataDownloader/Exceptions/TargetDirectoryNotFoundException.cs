using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Exceptions
{
    class TargetDirectoryNotFoundException : Exception
    {
        public TargetDirectoryNotFoundException(string message) : base(message)
        {

        }
    }
}
