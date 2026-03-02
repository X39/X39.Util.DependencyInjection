using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<MultipleConditionsOneFalseService>]
public class MultipleConditionsOneFalseService
{
    [DependencyInjectionCondition]
    public static bool ConditionA() => true;

    [DependencyInjectionCondition]
    public static bool ConditionB() => false;
}
