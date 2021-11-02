using DataDownloader.Exceptions;
using DataDownloader.Models;
using Microsoft.Extensions.Logging;
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
    public class GetDataCommand : Command, ICommand
    {
        private readonly TargetFolderSettings _settings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GetDataCommand> _logger;
        private readonly IIoReader _reader;
        private readonly IUrlVerifier _urlVerifier;

        public GetDataCommand(string name, 
            string description, 
            TargetFolderSettings settings, 
            IHttpClientFactory clientFactory, 
            ILogger<GetDataCommand> logger,
            IIoReader reader,
            IUrlVerifier urlVerifier) : base(name, description)
        {
            _settings = settings;
            _clientFactory = clientFactory;
            _logger = logger;
            _reader = reader;
            _urlVerifier = urlVerifier;
        }

        public override async Task ExecuteCommandAsync(IEnumerable<string> args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
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
                throw new TargetDirectoryNotFoundException("Directory doesn't exist"); 
            }

            Directory.CreateDirectory(directory);

            var urls = _reader.ReadUntil(";");
            if (args.Contains("-async"))
                await GetDataAsync(urls, directory);
            else
                await GetDataSync(urls, directory);
            watch.Stop();
            _logger.LogInformation($"Elapsed time [ms]: {watch.ElapsedMilliseconds}");
        }
        private async Task DownloadJsonFromUrl(string url, string directory, int fileNumber)
        {
            if (!_urlVerifier.IsUrlCorrect(url))
            {
                throw new UrlIncorrectException("Url has incorrect format.");
            }

            
            using var httpClient = _clientFactory.CreateClient();

            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();


                await using var fileStream = new FileStream(
                   Path.Combine(directory, $"{fileNumber}.json"),
                   FileMode.Create,
                   FileAccess.Write,
                   FileShare.None,
                   1024 * 80,
                   FileOptions.Asynchronous);
                await using var clientStream = await response
                    .Content
                    .ReadAsStreamAsync()
                    .ConfigureAwait(false);

                await clientStream
                    .CopyToAsync(fileStream)
                    .ConfigureAwait(false);
        }
        private async Task GetDataSync(IEnumerable<string> urls, string directory)
        {
            var tasks = new List<Task>();
            int i = 0;
            foreach (var url in urls)
            {

                await Task.Run(async () =>
                {
                    try
                    {
                        await DownloadJsonFromUrl(url.Trim(), directory, i);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Cannot get data from that url: {url}");
                        _logger.LogError(ex.Message);
                    }
                });
                i++;
            }
            await Task.WhenAll(tasks);
        }
        private async Task GetDataAsync(IEnumerable<string> urls, string directory)
        {
            async Task DownloadAsync(string url, int i)
            {
                Console.WriteLine($"task nr {i} started.");
                await DownloadJsonFromUrl(url.Trim(), directory, i);
            }

            var tasks = urls.Select(DownloadAsync);

            await Task.WhenAll(tasks);
        }
    }
}
