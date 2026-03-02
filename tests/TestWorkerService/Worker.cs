using System.Diagnostics.Contracts;
using TestWorkerService.Singletons;

namespace TestWorkerService;

public class Worker : BackgroundService
{
    public Worker(IServiceProvider serviceProvider)
    {
        Contract.Assert(serviceProvider.GetService<SingletonDirect>() is not null);
        Contract.Assert(serviceProvider.GetService<ISingletonInterface>() is not null);
        Contract.Assert(serviceProvider.GetService<SingletonConditionalFalse>() is null);
        Contract.Assert(serviceProvider.GetService<SingletonConditionalTrue>() is not null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}