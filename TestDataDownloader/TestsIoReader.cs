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
        }
        public TestsIoReader()
        {

        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnThreeCorrectElements()
        {
            //arrange
            var stream = new CustomStream("Techno;Rock;Rap");
            var expectedOutput = new List<string>() { "Techno", "Rock", "Rap" };

            //act
            var ioReadings = IoReader.ReadUntil(
                ";",
                stream.Read,
                stream.IsEmpty);

            var readings = new List<string>();
            foreach(var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnTwoCorrectElements()
        {
            //arrange
            var stream = new CustomStream("Techno;Rock;");
            var expectedOutput = new List<string>() { "Techno", "Rock" };

            //act
            var ioReadings = IoReader.ReadUntil(
                ";",
                stream.Read,
                stream.IsEmpty);

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilSemicolon_ShouldReturnTwoCorrectElementsAndEmptyString()
        {
            //arrange
            var stream = new CustomStream("Techno;;Rock;");
            var expectedOutput = new List<string>() { "Techno", "", "Rock" };

            //act
            var ioReadings = IoReader.ReadUntil(
                ";",
                stream.Read,
                stream.IsEmpty);

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilTwoSemicolons_ShouldReturnThreeCorrectElements()
        {
            //arrange
            var stream = new CustomStream("Techno;;Rap;;Rock");
            var expectedOutput = new List<string>() { "Techno", "Rap", "Rock" };

            //act
            var ioReadings = IoReader.ReadUntil(
                ";;",
                stream.Read,
                stream.IsEmpty);

            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }

            //assert
            CollectionAssert.AreEqual(readings, expectedOutput);
        }
        [TestMethod]
        public void TestReadUntilTwoSemicolons_ShouldReturnTwoCorrectElements()
        {
            //arrange
            var stream = new CustomStream("Techno;;Rap;Rock");
            var expectedOutput = new List<string>() { "Techno", "Rap;Rock" };

            //act
            var ioReadings = IoReader.ReadUntil(
                ";;",
                stream.Read,
                stream.IsEmpty);

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
            var stream = new CustomStream("Techno;;;Rap;Rock");

            //act
            var ioReadings = IoReader.ReadUntil(
                ";;",
                stream.Read,
                stream.IsEmpty);
            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }
        }
        //It is expected that read method passed by user can handle empty input stream (ex. Console.Read() waits for input)
        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TestReadUntilTwoSemicolons_ShouldThrowInvalidOperationException()
        {
            //arrange
            var stream = new CustomStream("");

            //act
            var ioReadings = IoReader.ReadUntil(
                ";;",
                stream.Read,
                stream.IsEmpty);
            var readings = new List<string>();
            foreach (var reading in ioReadings) { readings.Add(reading); }
        }
        [TestMethod]
        public void TestReadToEndNotEmptyString_ShouldReturnString()
        {
            //arrange
            var stream = new CustomStream("Techno;;;Rap;Rock");
            string expectedReturn = "Techno;;;Rap;Rock";

            //act
            var ioReading = IoReader.ReadToEnd(
                stream.ReadToEnd,
                stream.IsEmpty);

            //assert
            Assert.AreEqual(ioReading, expectedReturn);
        }
        [TestMethod]
        public void TestReadToEndEmptyString_ShouldReturnEmptyString()
        {
            //arrange
            var stream = new CustomStream("");
            string expectedReturn = "";

            //act
            var ioReading = IoReader.ReadToEnd(
                stream.ReadToEnd,
                stream.IsEmpty);

            //assert
            Assert.AreEqual(ioReading, expectedReturn);
        }

    }
}
