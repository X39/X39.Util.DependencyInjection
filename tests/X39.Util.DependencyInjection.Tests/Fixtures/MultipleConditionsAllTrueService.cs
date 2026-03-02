using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<MultipleConditionsAllTrueService>]
public class MultipleConditionsAllTrueService
{
    [DependencyInjectionCondition]
    public static bool ConditionA() => true;

    [DependencyInjectionCondition]
    public static bool ConditionB() => true;
}
