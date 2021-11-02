using DataDownloader;
using Microsoft.Extensions.DependencyInjection;
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
    public class TestsCommandParser
    {
        [TestMethod]
        public void TestGetArguments_ShouldReturnTwoArguments()
        {
            //arrange
            Mock<IIoReader> mockIoReader = new ();
            mockIoReader.Setup(x => x.ReadUntil(" ")).Returns(new List<string>()
            {
                "niewiem",
                "-wiem"
            });
            mockIoReader
                .SetupSequence(x => x.PeekIo())
                .Returns('-')
                .Returns(' ');

            var expectedOutput = new List<string>()
            {
                "niewiem",
                "-wiem"
            };

            var commandParser = new CommandParser(mockIoReader.Object);

            //act
            var arguments = commandParser.GetArguments();

            //assert
            CollectionAssert.AreEquivalent(expectedOutput, arguments.ToList());
        }
        //for command parser arguments are actually flags (starting with '-')
        [TestMethod]
        public void TestGetArgumentsWithValuesFollowingFlags_ShouldReturnTwoArguments()
        {
            //arrange
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadUntil(" ")).Returns(new List<string>()
            {
                "niewiem",
                "-wiem",
                "tosiezdecyduj"
            });
            mockIoReader
                .SetupSequence(x => x.PeekIo())
                .Returns('-')
                .Returns(' ');

            var expectedOutput = new List<string>()
            {
                "niewiem",
                "-wiem"
            };

            var commandParser = new CommandParser(mockIoReader.Object);

            //act
            var arguments = commandParser.GetArguments();

            //assert
            CollectionAssert.AreEquivalent(expectedOutput, arguments.ToList());
        }
        //flags has to be set before values
        [TestMethod]
        public void TestGetArgumentsWithFlagsFollowingValues_ShouldReturnTwoArguments()
        {
            //arrange
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadUntil(" ")).Returns(new List<string>()
            {
                "niewiem",
                "-wiem",
                "tosiezdecyduj",
                "-ok"
            });
            mockIoReader
                .SetupSequence(x => x.PeekIo())
                .Returns('-')
                .Returns(' ');

            var expectedOutput = new List<string>()
            {
                "niewiem",
                "-wiem"
            };

            var commandParser = new CommandParser(mockIoReader.Object);

            //act
            var arguments = commandParser.GetArguments();

            //assert
            CollectionAssert.AreEquivalent(expectedOutput, arguments.ToList());
        }
        [TestMethod]
        public void TestGetArgumentsWitoutFlags_ShouldReturnOneArgument()
        {
            //arrange
            Mock<IIoReader> mockIoReader = new();
            mockIoReader.Setup(x => x.ReadUntil(" ")).Returns(new List<string>()
            {
                "niewiem",
                "wiem",
                "tosiezdecyduj",
                "ok"
            });
            mockIoReader
                .SetupSequence(x => x.PeekIo())
                .Returns(' ')
                .Returns(' ');

            var expectedOutput = new List<string>()
            {
                "niewiem"
            };

            var commandParser = new CommandParser(mockIoReader.Object);

            //act
            var arguments = commandParser.GetArguments();

            //assert
            CollectionAssert.AreEquivalent(expectedOutput, arguments.ToList());
        }
    }
}
