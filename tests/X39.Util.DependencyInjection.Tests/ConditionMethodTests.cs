using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X39.Util.DependencyInjection.Tests.Fixtures;

namespace X39.Util.DependencyInjection.Tests;

public class ConditionMethodTests
{
    private static IConfiguration BuildEmptyConfiguration()
        => new ConfigurationBuilder().Build();

    private static IConfiguration BuildConfigurationWith(Dictionary<string, string?> values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

    [Fact]
    public void ConditionReturningTrue_ServiceIsRegistered()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(ConditionalTrueService).Assembly);

        Assert.Contains(services, d => d.ServiceType == typeof(ConditionalTrueService));
    }

    [Fact]
    public void ConditionReturningFalse_ServiceIsNotRegistered()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(ConditionalFalseService).Assembly);

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ConditionalFalseService));
    }

    [Fact]
    public void ConditionWithIConfiguration_ReceivesConfig_EnabledTrue()
    {
        var services = new ServiceCollection();
        var config = BuildConfigurationWith(new Dictionary<string, string?>
        {
            { "ConditionalWithConfig:Enabled", "true" },
        });

        services.AddAttributedServicesOf(config, typeof(ConditionalWithConfigService).Assembly);

        Assert.Contains(services, d => d.ServiceType == typeof(ConditionalWithConfigService));
    }

    [Fact]
    public void ConditionWithIConfiguration_ReceivesConfig_EnabledFalse()
    {
        var services = new ServiceCollection();
        var config = BuildConfigurationWith(new Dictionary<string, string?>
        {
            { "ConditionalWithConfig:Enabled", "false" },
        });

        services.AddAttributedServicesOf(config, typeof(ConditionalWithConfigService).Assembly);

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ConditionalWithConfigService));
    }

    [Fact]
    public void MultipleConditions_AllTrue_ServiceIsRegistered()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(MultipleConditionsAllTrueService).Assembly);

        Assert.Contains(services, d => d.ServiceType == typeof(MultipleConditionsAllTrueService));
    }

    [Fact]
    public void MultipleConditions_OneFalse_ServiceIsNotRegistered()
    {
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        services.AddAttributedServicesOf(config, typeof(MultipleConditionsOneFalseService).Assembly);

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(MultipleConditionsOneFalseService));
    }
}
