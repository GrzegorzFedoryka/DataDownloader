using DataDownloader.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataDownloader
{
    public interface IUrlVerifier
    {
        bool IsUrlCorrect(string textToMatch);
        Task<bool> UrlExists(string url);
    }
    public class UrlVerifier : IUrlVerifier
    {
        private readonly ILogger<UrlVerifier> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly UrlRegex _regex;

        public UrlVerifier(UrlRegex regex, ILogger<UrlVerifier> logger, IHttpClientFactory clientFactory)
        {
            _regex = regex;
            _logger = logger;
            _clientFactory = clientFactory;
        }
        public bool IsUrlCorrect(string textToMatch)
        {
            Match match = Regex.Match(textToMatch.Trim(), _regex.Regex, RegexOptions.Compiled);
            return match.Success;
        }
        public async Task<bool> UrlExists(string url)
        {
            try
            {
                using var httpClient = _clientFactory.CreateClient();
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception!");
                //Any exception will returns false.
                return false;
            }
        }
    }
}
