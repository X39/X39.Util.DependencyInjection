namespace X39.Util.DependencyInjection.Attributes;
using JetBrains.Annotations;

#if NET7_0_OR_GREATER
/// <summary>
/// Special attribute to mark a class as scoped.
/// </summary>
/// <remarks>
/// This attribute cannot be inherited.
/// </remarks>
/// <example>
/// <code>
///     public interface IMyConditionalService
///     {
///         bool SomeFunc();
///     }
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;]
///     public class MyConditionalService : IMyConditionalService
///     {
///         [DependencyInjectionCondition]
///         private static bool Condition()
///         {
///             #if DEBUG
///             return true;
///             #else
///             return false;
///             #endif
///         }
///         public bool SomeFunc(){
///             return true;
///         }
///     }
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;]
///     public class MyConditionalServiceMock : IMyConditionalService
///     {
///         [DependencyInjectionCondition]
///         private static bool Condition()
///         {
///             #if DEBUG
///             return false;
///             #else
///             return true;
///             #endif
///         }
///         public bool SomeFunc(){
///             return true;
///         }
///     }
/// </code>
/// </example>
[PublicAPI]
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ScopedAttribute<TService, TAbstraction>: Attribute, IAbstractedDependencyInjectionAttribute where TService : TAbstraction
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TAbstraction);

    /// <inheritdoc/>
    public Type ActualType { get; } = typeof(TService);
    
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Scoped;
}

/// <summary>
/// Special attribute to mark a class as scoped.
/// </summary>
/// <remarks>
/// This attribute cannot be inherited.
/// </remarks>
/// <example>
/// <code>
///     [Scoped&lt;MyNotAbstractedService&gt;]
///     public class MyNotAbstractedService
///     {
///         public bool SomeFunc(){
///             return true;
///         }
///     }
/// </code>
/// </example>
[PublicAPI]
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ScopedAttribute<TService> : Attribute, IDependencyInjectionAttribute
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TService);

    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Scoped;
}
#endif
/// <summary>
/// Special attribute to mark a class as scoped.
/// </summary>
/// <remarks>
/// This attribute cannot be inherited.
/// </remarks>
/// <example>
/// <code>
///     public interface IMyConditionalService
///     {
///         bool SomeFunc();
///     }
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;]
///     public class MyConditionalService : IMyConditionalService
///     {
///         [DependencyInjectionCondition]
///         private static bool Condition()
///         {
///             #if DEBUG
///             return true;
///             #else
///             return false;
///             #endif
///         }
///         public bool SomeFunc(){
///             return true;
///         }
///     }
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;]
///     public class MyConditionalServiceMock : IMyConditionalService
///     {
///         [DependencyInjectionCondition]
///         private static bool Condition()
///         {
///             #if DEBUG
///             return false;
///             #else
///             return true;
///             #endif
///         }
///         public bool SomeFunc(){
///             return true;
///         }
///     }
/// </code>
/// </example>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
#if NET7_0_OR_GREATER
[Obsolete("Use the generic typed version instead")]
#endif
public class ScopedAttribute : Attribute, IAbstractedDependencyInjectionAttribute
{
    /// <inheritdoc />
    public Type ServiceType { get; }

    /// <inheritdoc />
    public Type ActualType { get; }

    /// <summary>
    /// Creates an abstracted scoped service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> that is supposed to be implemented by the attached class.
    /// </param>
    /// <param name="actualType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public ScopedAttribute(Type serviceType, Type actualType)
    {
        ServiceType = serviceType;
        ActualType = actualType;
    }

    /// <summary>
    /// Creates an abstracted scoped service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public ScopedAttribute(Type serviceType)
    {
        ServiceType = serviceType;
        ActualType = serviceType;
    }
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Scoped;
}