using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WordStreamAnalyser.Application.Interfaces;
using WordStreamAnalyser.Domain;

namespace WordStreamAnalyser.Infrastructure
{
    public class ConsoleReportWriter(ILoggerFactory loggerFactory) : IReportWriter
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger("ConsoleReportWriter");

        public void WriteReport(StreamStatistics stats)
        {
            _logger.LogInformation("Writing to the console: {WordCount} words, {CharCount} characters",
                stats.TotalWords, stats.TotalChars);

            Console.Out.Flush();

            Console.WriteLine("\n--- Stream Statistics ---");
            Console.WriteLine($"Total Characters: {stats.TotalChars}");
            Console.WriteLine($"Total Words: {stats.TotalWords}");

            Console.WriteLine("\nTop 5 Largest Words:");

            foreach (var word in stats.LargestWords)
            {
                Console.WriteLine($"{word} ({word.Length})");
            }

            Console.WriteLine("\nTop 5 Smallest Words:");

            foreach (var word in stats.SmallestWords)
            {
                Console.WriteLine($"{word} ({word.Length})");
            }

            Console.WriteLine("\nTop 10 Most Frequent Words:");

            foreach (var word in stats.WordFrequency.OrderByDescending(w => w.Value).ThenBy(w => w.Key).Take(10))
            {
                Console.WriteLine($"{word.Key}: {word.Value}");
            }

            Console.WriteLine("\nCharacter Frequencies:");

            foreach (var entry in stats.CharFrequency.OrderByDescending(c => c.Value).ThenBy(c => c.Key))
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
        }
    }
}
