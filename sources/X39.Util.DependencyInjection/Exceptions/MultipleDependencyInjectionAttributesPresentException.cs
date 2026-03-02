using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when more then one dependency injection attribute is present for a given <see langword="class"/>.
/// </summary>
[PublicAPI]
public class MultipleDependencyInjectionAttributesPresentException : DependencyInjectionException
{
    /// <summary>
    /// The affected type.
    /// </summary>
    public Type Type { get; }
    
    /// <summary>
    /// The dependency injection attributes found.
    /// </summary>
    public IReadOnlyCollection<Attribute> Attributes { get; }

    internal MultipleDependencyInjectionAttributesPresentException(Type type, IReadOnlyCollection<Attribute> attributes)
        : base($"The type {type.FullName()} has multiple dependency injection attributes ({string.Join(", ", attributes.Select((q) => q.GetType().Name))}) set.")
    {
        Type = type;
        Attributes = attributes;
    }

    /// <inheritdoc />
    public MultipleDependencyInjectionAttributesPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Type = (Type) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Type),
            typeof(Type))!;
        Attributes = (IReadOnlyCollection<Attribute>) info.GetValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Attributes),
            typeof(Attribute[]))!;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Type),
            Type);
        info.AddValue(
            nameof(MultipleDependencyInjectionAttributesPresentException) + "." + nameof(Attributes),
            Attributes.ToArray());
    }
}