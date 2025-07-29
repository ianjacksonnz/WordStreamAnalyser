using Microsoft.Extensions.DependencyInjection;
using WordStreamAnalyser.Application.Interfaces;

namespace WordStreamAnalyser.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWordStream, BoosterWordStreamAdapter>();
        services.AddSingleton<IReportWriter, ConsoleReportWriter>();
        return services;
    }
}
