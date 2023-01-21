namespace X39.Util.DependencyInjection.Attributes;using JetBrains.Annotations;


#if NET7_0_OR_GREATER
/// <summary>
/// Special attribute to mark a class as singleton.
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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;]
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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;]
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
public sealed class SingletonAttribute<TService, TAbstraction> : Attribute, IAbstractedDependencyInjectionAttribute where TService : TAbstraction
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TAbstraction);

    /// <inheritdoc/>
    public Type ActualType { get; } = typeof(TService);
    
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Singleton;
}

/// <summary>
/// Special attribute to mark a class as singleton.
/// </summary>
/// <remarks>
/// This attribute cannot be inherited.
/// </remarks>
/// <example>
/// <code>
///     [Singleton&lt;MyNotAbstractedService&gt;]
///     public class MyNotAbstractedService
///     {
///         public bool SomeFunc(){
///             return true;
///         }
///     }
/// </code>
/// </example>
[PublicAPI]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SingletonAttribute<TService> : Attribute, IDependencyInjectionAttribute
{
    /// <inheritdoc/>
    public Type ServiceType { get; } = typeof(TService);

    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Singleton;
}
#endif
/// <summary>
/// Special attribute to mark a class as singleton.
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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;]
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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;]
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
public class SingletonAttribute : Attribute, IAbstractedDependencyInjectionAttribute
{
    /// <inheritdoc />
    public Type ServiceType { get; }

    /// <inheritdoc />
    public Type ActualType { get; }

    /// <summary>
    /// Creates an abstracted singleton service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> that is supposed to be implemented by the attached class.
    /// </param>
    /// <param name="actualType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public SingletonAttribute(Type serviceType, Type actualType)
    {
        ServiceType = serviceType;
        ActualType = actualType;
    }

    /// <summary>
    /// Creates an abstracted singleton service.
    /// </summary>
    /// <param name="serviceType">
    ///     <see cref="Type"/> of the class this <see cref="Attribute"/> is attached to.
    /// </param>
    public SingletonAttribute(Type serviceType)
    {
        ServiceType = serviceType;
        ActualType = serviceType;
    }
    EDependencyInjectionKind IDependencyInjectionAttribute.Kind { get; } = EDependencyInjectionKind.Singleton;
}