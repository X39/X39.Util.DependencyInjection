using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<ConditionalFalseService>]
public class ConditionalFalseService
{
    [DependencyInjectionCondition]
    public static bool IsEnabled() => false;
}
