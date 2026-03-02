using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace X39.Util.DependencyInjection.Exceptions;

/// <summary>
/// Base <see cref="Exception"/> class for the X39.Util.DependencyInjection library.
/// </summary>
[PublicAPI]
public abstract class DependencyInjectionException : Exception
{
    /// <inheritdoc />
    protected internal DependencyInjectionException()
    {
    }

    /// <inheritdoc />
    protected internal DependencyInjectionException(SerializationInfo info, StreamingContext context) : base(info,
        context)
    {
    }

    /// <inheritdoc />
    protected internal DependencyInjectionException(string? message) : base(message)
    {
    }

    /// <inheritdoc />
    protected internal DependencyInjectionException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }
}