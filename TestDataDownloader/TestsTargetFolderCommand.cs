using DataDownloader;
using DataDownloader.Commands;
using DataDownloader.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataDownloader
{
    [TestClass]
    public class TestsTargetFolderCommand
    {
        [TestMethod]
        public async Task TestExecuteCommandAsync_SetDirectory()
        {
            //arrange
            var settings = new TargetFolderSettings();
            var expectedSettings = new TargetFolderSettings() { Directory = "SomeDirectory", IsRelative = false, IsToCreateDirectiory = false };
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadToEnd()).Returns("SomeDirectory");

            //act
            var command = new TargetFolderCommand("targetfolder", "Description", settings, mockIoReader.Object);
            await command.ExecuteCommandAsync(new List<string>());

            //assert
            Assert.AreEqual(settings.Directory, expectedSettings.Directory);
            Assert.AreEqual(settings.IsRelative, expectedSettings.IsRelative);
            Assert.AreEqual(settings.IsToCreateDirectiory, expectedSettings.IsToCreateDirectiory);
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_SetDirectoryWithIsRelative()
        {
            //arrange
            var settings = new TargetFolderSettings();
            var expectedSettings = new TargetFolderSettings() { Directory = "SomeOtherDirectory", IsRelative = true, IsToCreateDirectiory = false };
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadToEnd()).Returns("SomeOtherDirectory");

            //act
            var command = new TargetFolderCommand("targetfolder", "Description", settings, mockIoReader.Object);
            await command.ExecuteCommandAsync(new List<string>() { "-r" });

            //assert
            Assert.AreEqual(settings.Directory, expectedSettings.Directory);
            Assert.AreEqual(settings.IsRelative, expectedSettings.IsRelative);
            Assert.AreEqual(settings.IsToCreateDirectiory, expectedSettings.IsToCreateDirectiory);
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_SetDirectoryWithFolderCreation()
        {
            //arrange
            var settings = new TargetFolderSettings();
            var expectedSettings = new TargetFolderSettings() { Directory = "SomeDirectory3", IsRelative = false, IsToCreateDirectiory = true };
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadToEnd()).Returns("SomeDirectory3");

            //act
            var command = new TargetFolderCommand("targetfolder", "Description", settings, mockIoReader.Object);
            await command.ExecuteCommandAsync(new List<string>() { "-c" });

            //assert
            Assert.AreEqual(settings.Directory, expectedSettings.Directory);
            Assert.AreEqual(settings.IsRelative, expectedSettings.IsRelative);
            Assert.AreEqual(settings.IsToCreateDirectiory, expectedSettings.IsToCreateDirectiory);
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_SetDirectoryWithFolderCreationAndIsRelative()
        {
            //arrange
            var settings = new TargetFolderSettings();
            var expectedSettings = new TargetFolderSettings() { Directory = "SomeDirectory4", IsRelative = true, IsToCreateDirectiory = true };
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadToEnd()).Returns("SomeDirectory4");

            //act
            var command = new TargetFolderCommand("targetfolder", "Description", settings, mockIoReader.Object);
            await command.ExecuteCommandAsync(new List<string>() { "-c", "-r" });

            //assert
            Assert.AreEqual(settings.Directory, expectedSettings.Directory);
            Assert.AreEqual(settings.IsRelative, expectedSettings.IsRelative);
            Assert.AreEqual(settings.IsToCreateDirectiory, expectedSettings.IsToCreateDirectiory);
        }
    }
}
