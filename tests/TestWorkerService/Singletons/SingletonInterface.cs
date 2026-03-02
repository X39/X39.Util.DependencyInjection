using X39.Util.DependencyInjection.Attributes;

namespace TestWorkerService.Singletons;

[Singleton<SingletonInterface, ISingletonInterface>]
public class SingletonInterface : ISingletonInterface
{
    public void Foo()
    {
        
    }
}