namespace X39.Util.DependencyInjection.Attributes;

internal interface IDependencyInjectionAttribute
{
    Type ServiceType { get; }
}