using Microsoft.Extensions.Configuration;
using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<ConditionalWithConfigService>]
public class ConditionalWithConfigService
{
    [DependencyInjectionCondition]
    public static bool IsEnabled(IConfiguration configuration)
        => string.Equals(configuration["ConditionalWithConfig:Enabled"], "true", StringComparison.OrdinalIgnoreCase);
}
