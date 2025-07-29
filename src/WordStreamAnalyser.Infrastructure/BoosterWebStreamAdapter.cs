using System;
using System.IO;
using WordStreamAnalyser.Application.Interfaces;

namespace WordStreamAnalyser.Infrastructure;

public class BoosterWordStreamAdapter : IWordStream
{
    private readonly Booster.CodingTest.Library.WordStream _inner;

    public BoosterWordStreamAdapter()
    {
        _inner = new Booster.CodingTest.Library.WordStream();
    }

    public StreamReader CreateReader() => new(_inner);

    public void Dispose()
    {
        _inner.Dispose();
        GC.SuppressFinalize(this);
    }
}
