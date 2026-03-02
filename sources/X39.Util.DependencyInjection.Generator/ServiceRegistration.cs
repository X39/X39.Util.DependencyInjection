using System;

namespace X39.Util.DependencyInjection.Generator;

/// <summary>
/// Data model for a single service registration discovered by the generator.
/// Must implement <see cref="IEquatable{T}"/> for incremental caching.
/// </summary>
internal sealed class ServiceRegistration : IEquatable<ServiceRegistration>
{
    public string FullyQualifiedClassName { get; }
    public string FullyQualifiedServiceType { get; }
    public bool HasAbstraction { get; }
    public string Lifetime { get; }
    public EquatableArray<ConditionMethod> ConditionMethods { get; }

    public ServiceRegistration(
        string fullyQualifiedClassName,
        string fullyQualifiedServiceType,
        bool hasAbstraction,
        string lifetime,
        EquatableArray<ConditionMethod> conditionMethods)
    {
        FullyQualifiedClassName = fullyQualifiedClassName;
        FullyQualifiedServiceType = fullyQualifiedServiceType;
        HasAbstraction = hasAbstraction;
        Lifetime = lifetime;
        ConditionMethods = conditionMethods;
    }

    public bool Equals(ServiceRegistration? other)
    {
        if (other is null) return false;
        return FullyQualifiedClassName == other.FullyQualifiedClassName
            && FullyQualifiedServiceType == other.FullyQualifiedServiceType
            && HasAbstraction == other.HasAbstraction
            && Lifetime == other.Lifetime
            && ConditionMethods.Equals(other.ConditionMethods);
    }

    public override bool Equals(object? obj) => obj is ServiceRegistration other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = FullyQualifiedClassName.GetHashCode();
            hash = hash * 31 + FullyQualifiedServiceType.GetHashCode();
            hash = hash * 31 + HasAbstraction.GetHashCode();
            hash = hash * 31 + Lifetime.GetHashCode();
            hash = hash * 31 + ConditionMethods.GetHashCode();
            return hash;
        }
    }
}
