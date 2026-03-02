using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X39.Util.DependencyInjection.Tests.Fixtures;

namespace X39.Util.DependencyInjection.Tests;

public class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildEmptyConfiguration()
        => new ConfigurationBuilder().Build();

    [Fact]
    public void RegistersSingleton_WithCorrectLifetime()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(SimpleSingleton).Assembly);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(SimpleSingleton));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void RegistersTransient_WithCorrectLifetime()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(SimpleTransient).Assembly);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(SimpleTransient));
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
    }

    [Fact]
    public void RegistersScoped_WithCorrectLifetime()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(SimpleScoped).Assembly);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(SimpleScoped));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void RegistersAbstractedService_WithInterfaceAsServiceType()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(AbstractedSingleton).Assembly);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IAbstractedService));
        Assert.Equal(typeof(AbstractedSingleton), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddAttributedServicesFromAssemblyOf_FindsServices()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesFromAssemblyOf<SimpleSingleton>(config);

        Assert.Contains(services, d => d.ServiceType == typeof(SimpleSingleton));
        Assert.Contains(services, d => d.ServiceType == typeof(SimpleTransient));
        Assert.Contains(services, d => d.ServiceType == typeof(SimpleScoped));
    }
}
