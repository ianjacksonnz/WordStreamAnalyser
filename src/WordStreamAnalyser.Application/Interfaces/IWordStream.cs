using System;
using System.IO;

namespace WordStreamAnalyser.Application.Interfaces;

public interface IWordStream : IDisposable
{
    StreamReader CreateReader();
}