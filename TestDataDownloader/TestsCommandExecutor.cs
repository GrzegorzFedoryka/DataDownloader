using DataDownloader;
using DataDownloader.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataDownloader
{
    [TestClass]
    public class TestsCommandExecutor
    {

        public ICommand[] Commands { get; set; }
        public TestsCommandExecutor()
        {
            Commands = new ICommand[] {
                new ReturnNumberCommand("setnumber", "Description")
            };
        }
        public class ReturnNumberCommand : Command, ICommand
        {
            
            public ReturnNumberCommand(string name, string description) : base(name, description)
            {
                
            }
            public int Number { get; set; } = 0;
            public override Task ExecuteCommandAsync(IEnumerable<string> args)
            {
                Number = int.Parse(args.First());
                return Task.CompletedTask;
            }
        }
        [TestMethod]
        public async Task TestExecuteCommandAsync_ShouldSetCorrectNumber()
        {
            //arrange
            var mockCommandSeeder = new Mock<ICommandSeeder>();
            var mockLogger = new Mock<ILogger<CommandExecutor>>();

            mockCommandSeeder.Setup(x => x.GetCommands()).Returns(Commands);
            var executor = new CommandExecutor(mockLogger.Object, mockCommandSeeder.Object);

            //act
            await executor.ExecuteCommandAsync(new List<string>() { "setnumber", "1" });

            var command = (ReturnNumberCommand)Commands.First(x => x.Name == "setnumber");
            var result = command.Number;
            //assert
            Assert.AreEqual(1, result);
        }
    }
}
