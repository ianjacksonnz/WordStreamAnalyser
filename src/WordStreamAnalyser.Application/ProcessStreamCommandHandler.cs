using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WordStreamAnalyser.Domain;
using WordStreamAnalyser.Application.Constants;
using WordStreamAnalyser.Application.Interfaces;

namespace WordStreamAnalyser.Application;

public class ProcessStreamCommandHandler(
    IWordStream wordStream,
    IReportWriter reportWriter,
    ILoggerFactory loggerFactory) : IRequestHandler<ProcessStreamCommand, Unit>
{
    private readonly IWordStream _wordStream = wordStream;
    private readonly IReportWriter _reportWriter = reportWriter;
    private readonly ILogger _logger = loggerFactory.CreateLogger("ProcessStreamCommandHandler");

    public async Task<Unit> Handle(ProcessStreamCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting stream processing.");

        using var reader = _wordStream.CreateReader();

        var wordRegex = new Regex(@"\w+", RegexOptions.Compiled);
        var charFrequency = new ConcurrentDictionary<char, int>();
        var wordFrequency = new ConcurrentDictionary<string, int>();

        var smallestWords = new SortedSet<string>(Comparer<string>.Create((a, b) =>
        {
            int lenCompare = a.Length.CompareTo(b.Length);
            return lenCompare != 0 ? lenCompare : a.CompareTo(b);
        }));

        var largestWords = new SortedSet<string>(Comparer<string>.Create((a, b) =>
        {
            int lenCompare = b.Length.CompareTo(a.Length);
            return lenCompare != 0 ? lenCompare : a.CompareTo(b);
        }));

        long totalChars = 0;
        long totalWords = 0;
        int charCounter = 0;
        var buffer = new char[StreamProcessingConstants.BufferSize];

        while (!reader.EndOfStream)
        {
            int count = await reader.ReadAsync(buffer, 0, buffer.Length);

            if (count == 0)
            {
                break;
            }

            var chunk = new string(buffer, 0, count);
            totalChars += count;
            charCounter += count;

            foreach (var c in chunk)
            {
                if (!char.IsWhiteSpace(c))
                {
                    charFrequency.AddOrUpdate(c, 1, (_, oldVal) => oldVal + 1);
                }
            }

            foreach (Match match in wordRegex.Matches(chunk))
            {
                string word = match.Value.ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(word))
                {
                    continue;
                }

                totalWords++;
                wordFrequency.AddOrUpdate(word, 1, (_, oldVal) => oldVal + 1);

                smallestWords.Add(word);

                if (smallestWords.Count > StreamProcessingConstants.MaxTrackedWords)
                {
                    smallestWords.Remove(smallestWords.Max);
                }

                largestWords.Add(word);

                if (largestWords.Count > StreamProcessingConstants.MaxTrackedWords)
                {
                    largestWords.Remove(largestWords.Max);
                }
            }

            if (charCounter >= StreamProcessingConstants.ReportIntervalTotalWords)
            {
                _logger.LogInformation("Writing intermediate report at {TotalWords} words and {TotalChars} characters", totalWords, totalChars);

                var stats = new StreamStatistics
                {
                    TotalChars = totalChars,
                    TotalWords = totalWords,
                    SmallestWords = smallestWords,
                    LargestWords = largestWords,
                    WordFrequency = wordFrequency,
                    CharFrequency = charFrequency
                };

                _reportWriter.WriteReport(stats);
                charCounter = 0;
            }
        }

        _logger.LogInformation("Writing final report at {TotalWords} words and {TotalChars} characters", totalWords, totalChars);

        var finalStats = new StreamStatistics
        {
            TotalChars = totalChars,
            TotalWords = totalWords,
            SmallestWords = smallestWords,
            LargestWords = largestWords,
            WordFrequency = wordFrequency,
            CharFrequency = charFrequency
        };

        _reportWriter.WriteReport(finalStats);

        _logger.LogInformation("Stream processing completed successfully.");

        return Unit.Value;
    }
}
