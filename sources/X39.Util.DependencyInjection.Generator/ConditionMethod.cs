using System;

namespace X39.Util.DependencyInjection.Generator;

/// <summary>
/// Represents a condition method decorated with <c>[DependencyInjectionCondition]</c>.
/// </summary>
internal sealed class ConditionMethod : IEquatable<ConditionMethod>
{
    public string Name { get; }
    public bool HasConfigurationParameter { get; }
    public bool IsAccessible { get; }

    public ConditionMethod(string name, bool hasConfigurationParameter, bool isAccessible)
    {
        Name = name;
        HasConfigurationParameter = hasConfigurationParameter;
        IsAccessible = isAccessible;
    }

    public bool Equals(ConditionMethod? other)
    {
        if (other is null) return false;
        return Name == other.Name
            && HasConfigurationParameter == other.HasConfigurationParameter
            && IsAccessible == other.IsAccessible;
    }

    public override bool Equals(object? obj) => obj is ConditionMethod other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = Name.GetHashCode();
            hash = hash * 31 + HasConfigurationParameter.GetHashCode();
            hash = hash * 31 + IsAccessible.GetHashCode();
            return hash;
        }
    }
}
