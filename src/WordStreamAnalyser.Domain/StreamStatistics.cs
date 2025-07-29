using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WordStreamAnalyser.Domain;

public record StreamStatistics
{
    public long TotalWords { get; init; }
    public long TotalChars { get; init; }
    public SortedSet<string> LargestWords { get; init; }
    public SortedSet<string> SmallestWords { get; init; }
    public ConcurrentDictionary<string, int> WordFrequency { get; init; }
    public ConcurrentDictionary<char, int> CharFrequency { get; init; }
}