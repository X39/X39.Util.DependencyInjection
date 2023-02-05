using TestWorkerService;
using X39.Util.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddAttributedServicesOf(context.Configuration, typeof(Program).Assembly);
    })
    .Build();

host.Run();