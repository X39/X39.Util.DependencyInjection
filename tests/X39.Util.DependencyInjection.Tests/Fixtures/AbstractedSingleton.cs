using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Tests.Fixtures;

[Singleton<AbstractedSingleton, IAbstractedService>]
public class AbstractedSingleton : IAbstractedService
{
    public string GetValue() => "abstracted";
}
