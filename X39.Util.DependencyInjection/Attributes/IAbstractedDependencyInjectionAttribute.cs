namespace X39.Util.DependencyInjection.Attributes;

internal interface IAbstractedDependencyInjectionAttribute : IDependencyInjectionAttribute
{
    string? ConditionMethod { get; }
    string? ConditionProperty { get; }
}