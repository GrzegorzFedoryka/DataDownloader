using DataDownloader;
using DataDownloader.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace TestDataDownloader
{
    [TestClass]
    public class TestsIoReader
    {
        private readonly ServiceProvider serviceProvider;
        private CustomStream Stream { set; get; } = new CustomStream("");
        public TestsIoReader()
        {
            var services = new ServiceCollection();
            var reader = new IoReader(Stream.Read,
                Stream.IsEmpty,
                Stream.ReadToEnd,
                Stream.Peek);

            services.AddSingleton<IIoReader>(reader);

            serviceProvider = services.BuildServiceProvider();
        }
        private class CustomStream
        {
            public Queue<char> StreamInput { get; set; }
            public CustomStream(string streamInput)
            {
                StreamInput = new Queue<char>();
                foreach(char c in streamInput) { this.StreamInput.Enqueue(c); }
            }
            public void AddToQueue(string text)
            {
                foreach (char c in text) { StreamInput.Enqueue(c); }
            }
            public char Read()
            {
                return StreamInput.Dequeue();
            }
            public bool IsEmpty()
            {
                return StreamInput.Count == 0;
            }
            public string ReadToEnd()
            {
                if (IsEmpty()) { return string.Empty; }
                var sb = new StringBuilder();
                while (!IsEmpty())
                {
                    sb.Append(Read());
                }
                return sb.ToString();
            }
            public char Peek()
            {
                return StreamInput.Peek();
            }
            public void Write(string text)
            {
                foreach (char c in text) { this.StreamInput.Enqueue(c); }
            }
            public void CleanInput()
            {
                while (!IsEmpty())
                {
                    StreamInput.Dequeue();
                }
            }
        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnThreeCorrectElements()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;Rock;Rap");
            var expectedOutput = new List<string>() { "Techno", "Rock", "Rap" };
            var reader = serviceProvider.GetRequiredService<IIoReader>();

            //act
            var ioReadings = reader.ReadUntil(";");

            var readings = new List<string>();
            foreach(var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnTwoCorrectElements()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;Rock;");
            var expectedOutput = new List<string>() { "Techno", "Rock" };
            var reader = serviceProvider.GetRequiredService<IIoReader>();

            //act
            var ioReadings = reader.ReadUntil(";");

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnTwoCorrectElementsAndEmptyString()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;;Rock;");
            var expectedOutput = new List<string>() { "Techno", "", "Rock" };
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReadings = reader.ReadUntil(";");

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilTwoSemicolons_ShouldReturnThreeCorrectElements()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;;Rap;;Rock");
            var expectedOutput = new List<string>() { "Techno", "Rap", "Rock" };
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReadings = reader.ReadUntil(";;");

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilTwoSemicolons_ShouldReturnTwoCorrectElements()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;;Rap;Rock");
            var expectedOutput = new List<string>() { "Techno", "Rap;Rock" };
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReadings = reader.ReadUntil(";;");

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        [ExpectedException(typeof(DelimitersOverlapException))]
        public void TestReadUntilTwoSemicolons_ShouldThrowDelimiterOverlapException()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;;;Rap;Rock");
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReadings = reader.ReadUntil(";;");
            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }
        }
        //It is expected that read method passed by user can handle empty input stream (ex. Console.Read() waits for input)
        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TestReadUntilTwoSemicolons_ShouldThrowInvalidOperationException()
        {
            //arrange
            Stream.CleanInput();
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReadings = reader.ReadUntil(";;");
            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }
        }
        [TestMethod]
        public void TestReadToEndNotEmptyString_ShouldReturnString()
        {
            //arrange
            Stream.CleanInput();
            Stream.Write("Techno;;;Rap;Rock");
            string expectedReturn = "Techno;;;Rap;Rock";
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReading = reader.ReadToEnd();

            //assert
            Assert.AreEqual(ioReading, expectedReturn);
        }
        [TestMethod]
        public void TestReadToEndEmptyString_ShouldReturnEmptyString()
        {
            //arrange
            Stream.CleanInput();
            string expectedReturn = "";
            var reader = serviceProvider.GetRequiredService<IIoReader>();
            //act
            var ioReading = reader.ReadToEnd();

            //assert
            Assert.AreEqual(ioReading, expectedReturn);
        }

    }
}
