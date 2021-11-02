using DataDownloader;
using DataDownloader.Commands;
using DataDownloader.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestDataDownloader
{
    [TestClass]
    public class TestsGetDataCommand
    {
        private readonly ServiceProvider serviceProvider;

        public TestsGetDataCommand()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            serviceProvider = services.BuildServiceProvider();
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_GetSync()
        {
            //arrange
            var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var settings = new TargetFolderSettings() { Directory = "NewFolderx", IsRelative = true, IsToCreateDirectiory = true };
            var mockUrlVerifier = new Mock<IUrlVerifier>();
            mockUrlVerifier.Setup(x => x.IsUrlCorrect(It.IsAny<string>())).Returns(true);
            var mockLogger = new Mock<ILogger<GetDataCommand>>();
            var mockIoReader = new Mock<IIoReader>();
            mockIoReader.Setup(x => x.ReadUntil(";")).Returns(new List<string>()
            {
                "https://api.publicapis.org/entries",
                "https://catfact.ninja/fact",
                "https://api.coindesk.com/v1/bpi/currentprice.json",
                "https://www.boredapi.com/api/activity",
                "https://api.agify.io?name=meelad",
                "https://api.genderize.io?name=luc"
            });

            var command = new GetDataCommand("getdata", "description", settings, clientFactory, mockLogger.Object, mockIoReader.Object, mockUrlVerifier.Object);

            //act
            await command.ExecuteCommandAsync(new List<string>());

            //assert
            int filesCount = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + settings.Directory, "*", SearchOption.TopDirectoryOnly).Length;
            Assert.AreEqual(filesCount, 6);
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_GetAsync()
        {
            //arrange
            var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var settings = new TargetFolderSettings() { Directory = "NewFoldery", IsRelative = true, IsToCreateDirectiory = true };
            var mockUrlVerifier = new Mock<IUrlVerifier>();
            mockUrlVerifier.Setup(x => x.IsUrlCorrect(It.IsAny<string>())).Returns(true);
            var mockLogger = new Mock<ILogger<GetDataCommand>>();
            var mockIoReader = new Mock<IIoReader>();
            mockIoReader.Setup(x => x.ReadUntil(";")).Returns(new List<string>()
            {
                "https://api.publicapis.org/entries",
                "https://catfact.ninja/fact",
                "https://api.coindesk.com/v1/bpi/currentprice.json",
                "https://www.boredapi.com/api/activity",
                "https://api.agify.io?name=meelad",
                "https://api.genderize.io?name=luc"
            });

            var command = new GetDataCommand("getdata", "description", settings, clientFactory, mockLogger.Object, mockIoReader.Object, mockUrlVerifier.Object);

            //act
            await command.ExecuteCommandAsync(new List<string>() { "-async" });

            //assert
            int filesCount = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + settings.Directory, "*", SearchOption.TopDirectoryOnly).Length;
            Assert.AreEqual(filesCount, 6);
        }
    }
}
