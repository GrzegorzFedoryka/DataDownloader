using DataDownloader.Commands;
using DataDownloader.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    interface ICommandSeeder
    {
        ICommand[] GetCommands();
    }

    class CommandSeeder : ICommandSeeder
    {
        private readonly TargetFolderSettings _settings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GetDataCommand> _getDataLogger;

        public CommandSeeder(TargetFolderSettings settings, IHttpClientFactory clientFactory, ILogger<GetDataCommand> getDataLogger)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _getDataLogger = getDataLogger;
        }
        public ICommand[] GetCommands()
        {
            ICommand[] commands =
            {
                new TargetFolderCommand("targetfolder", "set destination path for files. Flags: -c (set if you want to create directory), -r (set if setted directory is relative)", _settings),
                new GetDataCommand("getdata", "get data from passed URL separated with ;. Flags: -async (set if you want to download files concurrently. It is not safe, but faster)", _settings, _clientFactory, _getDataLogger)
            };
            return commands;
        }
    }
}
