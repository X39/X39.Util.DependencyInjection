using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a dependency injection <see cref="Attribute"/> is being used but the <see cref="System.Type"/>
/// given to inject is not implemented by the <see cref="System.Type"/> decorated with the <see cref="Attribute"/>.
/// </summary>
[PublicAPI]
public class ServiceTypeIsNotImplementingDecoratedTypeException : DependencyInjectionException
{
    /// <summary>
    /// The type that is decorated with a dependency injection attribute.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The type that is contracted in a dependency injection attribute.
    /// </summary>
    public Type ServiceType { get; }

    internal ServiceTypeIsNotImplementingDecoratedTypeException(Type type, Type serviceType)
        : base($"{type.FullName()} is not implementing the given {serviceType.FullName()} type.")
    {
        Type = type;
        ServiceType = serviceType;
    }

    /// <inheritdoc />
    public ServiceTypeIsNotImplementingDecoratedTypeException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
        Type = (Type) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Type),
            typeof(Type))!;
        ServiceType = (Type) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(ServiceType),
            typeof(Type))!;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Type),
            Type);
        info.AddValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(ServiceType),
            ServiceType);
    }
}