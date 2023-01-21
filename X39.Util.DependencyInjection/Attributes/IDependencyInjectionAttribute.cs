namespace X39.Util.DependencyInjection.Attributes;

internal interface IDependencyInjectionAttribute
{
    /// <summary>
    /// The service type that is implemented.
    /// </summary>
    Type ServiceType { get; }
    
    /// <summary>
    /// The kind of injection the attribute denotes.
    /// </summary>
    EDependencyInjectionKind Kind { get; }
}