using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataDownloader
{
    interface IUrlVerifier
    {
        bool IsUrlCorrect(string textToMatch);
        bool UrlExists(string url);
    }
    class UrlVerifier : IUrlVerifier
    {
        private readonly ILogger<UrlVerifier> _logger;

        public UrlRegex _regex;

        public UrlVerifier(UrlRegex regex, ILogger<UrlVerifier> logger)
        {
            _regex = regex;
            _logger = logger;
        }
        public bool IsUrlCorrect(string textToMatch)
        {
            Match match = Regex.Match(textToMatch.Trim(), _regex.Regex, RegexOptions.Compiled);
            return match.Success;
        }
        public bool UrlExists(string url)
        {
            try
            {
                var uri = GetUri(url.Trim());
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var statusCode = response.StatusCode;
                //Returns TRUE if the Status code == 200
                response.Close();
                return ((int)statusCode > 100 && (int)statusCode < 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception!");
                //Any exception will returns false.
                return false;
            }
        }
        private static Uri GetUri(string s)
        {
            return new UriBuilder(s).Uri;
        }
    }
}
