using DataDownloader.Commands;
using DataDownloader.Models;
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

        public CommandSeeder(TargetFolderSettings settings, IHttpClientFactory clientFactory)
        {
            _settings = settings;
            _clientFactory = clientFactory;
        }
        public ICommand[] GetCommands()
        {
            ICommand[] commands =
            {
                new TargetFolderCommand("targetfolder", "  ", _settings),
                new GetDataCommand("getdata", " ", _settings, _clientFactory)
            };
            return commands;
        }
    }
}
