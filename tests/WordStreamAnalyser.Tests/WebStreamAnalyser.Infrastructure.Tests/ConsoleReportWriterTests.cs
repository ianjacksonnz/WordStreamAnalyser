using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.IO;
using WordStreamAnalyser.Domain;
using WordStreamAnalyser.Infrastructure;
using Xunit;

namespace WordStreamAnalyser.Tests.WebStreamAnalyser.Infrastructure.Tests
{
    public class ConsoleReportWriterTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly ConsoleReportWriter _reportWriter;

        public ConsoleReportWriterTests()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _logger = Substitute.For<ILogger>();
            _loggerFactory.CreateLogger("ConsoleReportWriter").Returns(_logger);

            _reportWriter = new ConsoleReportWriter(_loggerFactory);
        }

        [Fact]
        public void WriteReport_LogsReportSummaryAndOutputsToConsole()
        {
            // Arrange
            var stats = new StreamStatistics
            {
                TotalWords = 20,
                TotalChars = 100,
                LargestWords = ["encyclopedia", "philosophy"],
                SmallestWords = ["a", "an"],
                WordFrequency = new()
                {
                    ["the"] = 10,
                    ["and"] = 5,
                    ["apple"] = 2
                },
                CharFrequency = new()
                {
                    ['a'] = 10,
                    ['b'] = 5
                }
            };

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            _reportWriter.WriteReport(stats);
            var output = consoleOutput.ToString();

            // Assert: Logger
            _logger.Received(1).Log(
                LogLevel.Information,
                0,
                Arg.Is<object>(o => o.ToString().Contains("Generating stream report")),
                null,
                Arg.Any<Func<object, Exception, string>>());

            _logger.Received(1).Log(
                LogLevel.Information,
                0,
                Arg.Is<object>(o => o.ToString().Contains("Stream report complete")),
                null,
                Arg.Any<Func<object, Exception, string>>());

            // Assert: Console Output
            output.Should().Contain("--- Stream Statistics ---");
            output.Should().Contain("Total Characters: 100");
            output.Should().Contain("Total Words: 20");
            output.Should().Contain("Top 5 Largest Words:");
            output.Should().Contain("encyclopedia (12)");
            output.Should().Contain("philosophy (10)");
            output.Should().Contain("Top 5 Smallest Words:");
            output.Should().Contain("a (1)");
            output.Should().Contain("an (2)");
            output.Should().Contain("Top 10 Most Frequent Words:");
            output.Should().Contain("the: 10");
            output.Should().Contain("and: 5");
            output.Should().Contain("apple: 2");
            output.Should().Contain("Character Frequencies:");
            output.Should().Contain("a: 10");
            output.Should().Contain("b: 5");
        }
    }
}
