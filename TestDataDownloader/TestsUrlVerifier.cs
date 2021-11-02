using DataDownloader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestDataDownloader
{
    [TestClass]
    public class TestsUrlVerifier
    {
        private readonly IConfiguration _configuration;
        private readonly UrlRegex _regex;
        private readonly ServiceProvider serviceProvider;
        public TestsUrlVerifier()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            _configuration = BuildConfiguration(path);
            _regex = new UrlRegex();
            _configuration.GetSection("UrlRegex").Bind(_regex);

            var services = new ServiceCollection();
            services.AddHttpClient();
            serviceProvider = services.BuildServiceProvider();
        }
        [TestMethod]
        public void TestIsUrlCorrect_IsCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger<UrlVerifier>>();
            var mockIHttpClientFactory = new Mock<IHttpClientFactory>();

            var urlVerifier = new UrlVerifier(_regex, mockLogger.Object, mockIHttpClientFactory.Object);

            //act
            var isCorrect = urlVerifier.IsUrlCorrect("www.itsOkay.com");

            //assert
            Assert.IsTrue(isCorrect);
        }
        [TestMethod]
        public void TestIsUrlCorrect_IsNotCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger<UrlVerifier>>();
            var mockIHttpClientFactory = new Mock<IHttpClientFactory>();

            var urlVerifier = new UrlVerifier(_regex, mockLogger.Object, mockIHttpClientFactory.Object);

            //act
            var isCorrect = urlVerifier.IsUrlCorrect("www.itsOka y.com");

            //assert
            Assert.IsFalse(isCorrect);
        }
        [TestMethod]
        public void TestIsUrlCorrect_IsCorrect2()
        {
            //arrange
            var mockLogger = new Mock<ILogger<UrlVerifier>>();
            var mockIHttpClientFactory = new Mock<IHttpClientFactory>();

            var urlVerifier = new UrlVerifier(_regex, mockLogger.Object, mockIHttpClientFactory.Object);

            //act
            var isCorrect = urlVerifier.IsUrlCorrect("https://www.itsSoOkay.com");

            //assert
            Assert.IsTrue(isCorrect);
        }
        [TestMethod]
        public async Task TestUrlExists_IsNotCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger<UrlVerifier>>();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var urlVerifier = new UrlVerifier(_regex, mockLogger.Object, httpClientFactory);

            //act
            var isCorrect = await urlVerifier.UrlExists("www.itsNotOkay.com");

            //assert
            Assert.IsFalse(isCorrect);
        }
        [TestMethod]
        public async Task TestUrlExists_IsCorrect()
        {
            //arrange
            var mockLogger = new Mock<ILogger<UrlVerifier>>();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            var urlVerifier = new UrlVerifier(_regex, mockLogger.Object, httpClientFactory);

            //act
            var isCorrect = await urlVerifier.UrlExists("https://api.publicapis.org/entries");

            //assert
            Assert.IsTrue(isCorrect);
        }
        private static IConfigurationRoot BuildConfiguration(string path)
        {
            return new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }
    }
}
