using DataDownloader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataDownloader.Commands
{
    class GetDataCommand : Command, ICommand
    {
        private readonly TargetFolderSettings _settings;
        private readonly IHttpClientFactory _clientFactory;

        public GetDataCommand(string name, string description, TargetFolderSettings settings, IHttpClientFactory clientFactory) : base(name, description)
        {
            _settings = settings;
            _clientFactory = clientFactory;
        }

        public override async Task ExecuteCommandAsync(IEnumerable<string> args)
        {
            StringBuilder directorySb = new();
            if(_settings.IsRelative == true)
            {
                directorySb.Append(AppDomain.CurrentDomain.BaseDirectory + _settings.Directory);
            }
            else
            {
                directorySb.Append(_settings.Directory);
            }
            var directory = directorySb.ToString();

            if(_settings.IsToCreateDirectiory == false && !Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("Directory doesn't exist"); 
            }

            Directory.CreateDirectory(directory);

            var urls = IoReader.ReadUntil(";",
                () => (char)Console.Read(),
                () => Console.In.Peek() == -1);
            int i = 0;
            var tasks = new List<Task>();
            foreach (var url in urls)
            {

                await Task.Run(async () => 
                {
                    try
                    {
                        await DownloadJsonFromUrl(url, directory, i);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                i++;
                Console.WriteLine($"task nr {i} started.");
            }
            //await Task.WhenAll(tasks);
            i = 0;
            foreach(var task in tasks)
            {
                Console.WriteLine($"{task.Exception} {i}");
                i++;
            }
            
        }
        private async Task DownloadJsonFromUrl(string url, string directory, int fileNumber)
        {
            using (var httpClient = _clientFactory.CreateClient())
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            using (FileStream fileStream = File.Open(directory + fileNumber.ToString() + ".json", FileMode.Create, FileAccess.Write, FileShare.None))
            using (var clientStream = await response.Content.ReadAsStreamAsync())
            {
                await clientStream.CopyToAsync(fileStream);
            }
        }
    }
}
