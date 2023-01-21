using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a dependency injection <see cref="Attribute"/> is being used but the <see cref="System.Type"/>
/// given for the actual implementation is not matching the one decorated with the <see cref="Attribute"/>.
/// </summary>
[PublicAPI]
public class ActualTypeIsNotMatchingDecoratedTypeException : DependencyInjectionException
{
    /// <summary>
    /// The type that is decorated with a dependency injection attribute.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The type that is abstracted in a dependency injection attribute.
    /// </summary>
    public Type ActualType { get; }

    internal ActualTypeIsNotMatchingDecoratedTypeException(Type type, Type actualType)
        : base($"{type.FullName()} is not matching the given {actualType.FullName()} type.")
    {
        Type = type;
        ActualType = actualType;
    }

    /// <inheritdoc />
    public ActualTypeIsNotMatchingDecoratedTypeException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
        Type = (Type) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Type),
            typeof(Type))!;
        ActualType = (Type) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(ActualType),
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
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(ActualType),
            ActualType);
    }
}