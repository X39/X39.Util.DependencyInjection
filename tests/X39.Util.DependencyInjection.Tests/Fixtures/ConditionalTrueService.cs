using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<ConditionalTrueService>]
public class ConditionalTrueService
{
    [DependencyInjectionCondition]
    public static bool IsEnabled() => true;
}
