using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when the library wants to add a dependency injected service but the class describing it is not
/// marked as <see langword="sealed"/>.
/// </summary>
[PublicAPI]
[Obsolete("This exception is not used anymore and will be removed in the future.")]
public class TypeNotSealedException : DependencyInjectionException
{
    internal TypeNotSealedException(Type type) : base($"The type {type.FullName()} is not sealed.")
    {
    }
}