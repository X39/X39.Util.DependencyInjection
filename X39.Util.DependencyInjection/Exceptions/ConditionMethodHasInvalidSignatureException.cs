using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Thrown when a condition method has an invalid signature.
/// </summary>
[PublicAPI]
public class ConditionMethodHasInvalidSignatureException : DependencyInjectionException
{
    /// <summary>
    /// The affected type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// The affected method.
    /// </summary>
    public MethodInfo AffectedMethod { get; }

    internal ConditionMethodHasInvalidSignatureException(Type type, MethodInfo methodInfo)
        : base($"The condition method {type.FullName()}.{methodInfo.Name} has an invalid signature.")
    {
        Type = type;
        AffectedMethod = methodInfo;
    }

    /// <inheritdoc />
    public ConditionMethodHasInvalidSignatureException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
        Type = (Type) info.GetValue(
            nameof(ConditionMethodHasInvalidSignatureException) + "." + nameof(Type),
            typeof(Type))!;
        AffectedMethod = (MethodInfo) info.GetValue(
            nameof(ConditionMethodHasInvalidSignatureException) + "." + nameof(AffectedMethod),
            typeof(MethodInfo))!;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(
            nameof(ConditionMethodHasInvalidSignatureException) + "." + nameof(Type),
            Type);
        info.AddValue(
            nameof(ConditionMethodHasInvalidSignatureException) + "." + nameof(AffectedMethod),
            AffectedMethod);
    }
}