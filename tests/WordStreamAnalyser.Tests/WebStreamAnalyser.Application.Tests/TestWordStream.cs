using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WordStreamAnalyser.Application.Interfaces;

namespace WordStreamAnalyser.Tests.WebStreamAnalyser.Application.Tests;

public class TestWordStream(string content) : IWordStream
{
    private readonly MemoryStream _innerStream = new(System.Text.Encoding.UTF8.GetBytes(content));

    public StreamReader CreateReader() => new(_innerStream);

    public int Read(byte[] buffer, int offset, int count) =>
        _innerStream.Read(buffer, offset, count);

    public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

    public bool CanRead => _innerStream.CanRead;
    public bool CanSeek => _innerStream.CanSeek;
    public long Length => _innerStream.Length;

    public long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public void Dispose()
    {
        _innerStream.Dispose();
        GC.SuppressFinalize(this);
    }
}
