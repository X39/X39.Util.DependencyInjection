namespace X39.Util.DependencyInjection.Attributes;using JetBrains.Annotations;


#if NET7_0_OR_GREATER
/// <summary>
/// Special attribute to mark a class as transient.
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
///     [Transient&lt;MyConditionalService, IMyConditionalService&gt;]
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
///     [Transient&lt;MyConditionalService, IMyConditionalService&gt;]
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
public sealed class TransientAttribute<TService, TAbstraction> : Attribute, IAbstractedDependencyInjectionAttribute where TService : TAbstraction
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TAbstraction);

    /// <inheritdoc/>
    public Type ActualType { get; } = typeof(TService);
    
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Transient;
}

/// <summary>
/// Special attribute to mark a class as transient.
/// </summary>
/// <remarks>
/// This attribute cannot be inherited.
/// </remarks>
/// <example>
/// <code>
///     [Transient&lt;MyNotAbstractedService&gt;]
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
public sealed class TransientAttribute<TService> : Attribute, IDependencyInjectionAttribute
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TService);

    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Transient;
}
#endif
/// <summary>
/// Special attribute to mark a class as transient.
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
///     [Transient(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
///     public class MyConditionalService : IMyConditionalService
///     {
///     #if DEBUG
///         private static bool IsLoaded => true;
///     #else
///         private static bool IsLoaded => false;
///     #endif
///         public bool SomeFunc(){
///             return true;
///         }
///     }
///     [Transient(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
///     public class MyConditionalServiceMock : IMyConditionalService
///     {
///     #if DEBUG
///         private static bool IsLoaded => false;
///     #else
///         private static bool IsLoaded => true;
///     #endif
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
public class TransientAttribute : Attribute, IAbstractedDependencyInjectionAttribute
{
    /// <inheritdoc />
    public Type ServiceType { get; }

    /// <inheritdoc />
    public Type ActualType { get; }

    /// <summary>
    /// Creates an abstracted transient service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> that is supposed to be implemented by the attached class.
    /// </param>
    /// <param name="actualType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public TransientAttribute(Type serviceType, Type actualType)
    {
        ServiceType = serviceType;
        ActualType = actualType;
    }

    /// <summary>
    /// Creates an abstracted transient service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public TransientAttribute(Type serviceType)
    {
        ServiceType = serviceType;
        ActualType = serviceType;
    }
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Transient;
}