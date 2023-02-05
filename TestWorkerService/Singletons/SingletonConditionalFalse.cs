using System.Diagnostics.CodeAnalysis;
using X39.Util.DependencyInjection.Attributes;

namespace TestWorkerService.Singletons;

[Singleton<SingletonConditionalFalse>]
public class SingletonConditionalFalse
{
    
    [DependencyInjectionCondition]
    private static bool DiCondition([SuppressMessage("ReSharper", "UnusedParameter.Local")]IConfiguration configuration)
    {
        return false;
    }
}