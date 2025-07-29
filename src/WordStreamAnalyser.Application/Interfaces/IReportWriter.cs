namespace WordStreamAnalyser.Application.Interfaces;

public interface IReportWriter
{
    void WriteReport(Domain.StreamStatistics stats);
}