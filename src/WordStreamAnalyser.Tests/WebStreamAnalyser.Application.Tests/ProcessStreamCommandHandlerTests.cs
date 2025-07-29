using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WordStreamAnalyser.Application;
using WordStreamAnalyser.Application.Constants;
using WordStreamAnalyser.Application.Interfaces;
using WordStreamAnalyser.Domain;
using Xunit;

namespace WordStreamAnalyser.Tests.WebStreamAnalyser.Application.Tests;

public class ProcessStreamCommandHandlerTests
{
    private static (ProcessStreamCommandHandler handler, IReportWriter reportWriter, ILogger<ProcessStreamCommandHandler> logger) CreateHandler(string testText)
    {
        var wordStream = new TestWordStream(testText);
        var reportWriter = Substitute.For<IReportWriter>();

        var logger = Substitute.For<ILogger<ProcessStreamCommandHandler>>();
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);

        var handler = new ProcessStreamCommandHandler(wordStream, reportWriter, loggerFactory);

        return (handler, reportWriter, logger);
    }

    [Fact]
    public async Task Handler_Should_Process_Word_Stream_And_Call_ReportWriter()
    {
        // Arrange
        var testText = "Hello world! Hello universe. Cats, dogs, and birds.";
        var (handler, reportWriter, logger) = CreateHandler(testText);

        // Act
        await handler.Handle(new ProcessStreamCommand(), CancellationToken.None);

        // Assert
        reportWriter.Received(1).WriteReport(Arg.Do<StreamStatistics>(s =>
        {
            s.TotalWords.Should().Be(8);
            s.TotalChars.Should().Be(testText.Length);
            s.WordFrequency.Should().ContainKey("hello").WhoseValue.Should().Be(2);
            s.WordFrequency.Should().ContainKey("world").WhoseValue.Should().Be(1);
        }));

        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }

    [Fact]
    public async Task Handler_Should_Handle_Empty_Input_Gracefully()
    {
        // Arrange
        var (handler, reportWriter, _) = CreateHandler("");

        // Act
        await handler.Handle(new ProcessStreamCommand(), CancellationToken.None);

        // Assert
        reportWriter.Received(1).WriteReport(Arg.Do<StreamStatistics>(s =>
        {
            s.TotalWords.Should().Be(0);
            s.TotalChars.Should().Be(0);
            s.WordFrequency.Should().BeEmpty();
        }));
    }

    [Fact]
    public async Task Handler_Should_Generate_Intermediate_Reports_When_Threshold_Exceeded()
    {
        // Arrange
        var chunk = "word ";
        var repeat = StreamProcessingConstants.ReportIntervalTotalWords / chunk.Length + 1;
        var testText = string.Concat(Enumerable.Repeat(chunk, repeat)); // > 1_000_000 chars
        var (handler, reportWriter, _) = CreateHandler(testText);

        // Act
        await handler.Handle(new ProcessStreamCommand(), CancellationToken.None);

        // Assert
        // Should be at least 2 reports (intermediate + final)
        reportWriter.ReceivedWithAnyArgs(2).WriteReport(default);
    }

    [Fact]
    public async Task Handler_Should_Track_Smallest_And_Largest_Words_Correctly()
    {
        // Arrange
        var testText = "a an ant apple appliance appreciation acknowledgement abracadabra amazing artifacts";
        var (handler, reportWriter, _) = CreateHandler(testText);

        // Act
        await handler.Handle(new ProcessStreamCommand(), CancellationToken.None);

        // Assert
        reportWriter.Received(1).WriteReport(Arg.Do<StreamStatistics>(s =>
        {
            s.SmallestWords.Should().Contain(["a", "an", "ant", "apple", "amazing"]);
            s.LargestWords.Should().Contain(word => word.Length >= 9);
            s.SmallestWords.Should().HaveCount(c => c <= StreamProcessingConstants.MaxTrackedWords);
            s.LargestWords.Should().HaveCount(c => c <= StreamProcessingConstants.MaxTrackedWords);
        }));
    }
}
