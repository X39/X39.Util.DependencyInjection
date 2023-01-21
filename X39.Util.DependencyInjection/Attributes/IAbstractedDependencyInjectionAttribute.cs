namespace X39.Util.DependencyInjection.Attributes;

internal interface IAbstractedDependencyInjectionAttribute : IDependencyInjectionAttribute
{
    /// <summary>
    /// The actual class that implements the <see cref="IDependencyInjectionAttribute.ServiceType"/>
    /// </summary>
    Type ActualType { get; }
}