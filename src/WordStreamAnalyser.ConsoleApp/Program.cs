using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using WordStreamAnalyser.Application;
using WordStreamAnalyser.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
            options.IncludeScopes = false;
        });
    })
    .ConfigureServices((context, services) =>
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProcessStreamCommand).Assembly));
        services.AddInfrastructure();
    })
    .Build();

var mediator = host.Services.GetRequiredService<IMediator>();
await mediator.Send(new ProcessStreamCommand());
