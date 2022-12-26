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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;(ConditionProperty = nameof(IsLoaded))]
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
///     [Singleton&lt;MyConditionalService, IMyConditionalService&gt;(ConditionProperty = nameof(IsLoaded))]
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
// ReSharper disable once UnusedTypeParameter
public sealed class SingletonAttribute<TService, TAbstraction> : Attribute, IAbstractedDependencyInjectionAttribute where TService : TAbstraction
{
    /// <summary>
    /// The service type that is implemented with the Singleton.
    /// </summary>
    public Type ServiceType { get; } = typeof(TAbstraction);

    /// <summary>
    /// An optional method serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool SingletonCondition();
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool SingletonCondition();
    /// </code>
    /// </example>
    public string? ConditionMethod { get; set; }

    /// <summary>
    /// An optional property serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool SingletonCondition { get; }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool SingletonCondition { get; }
    /// </code>
    /// </example>
    public string? ConditionProperty { get; set; }
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
    /// <summary>
    /// The service type that is implemented with the Singleton.
    /// </summary>
    public Type ServiceType { get; } = typeof(TService);
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
///     [Singleton(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
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
///     [Singleton(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
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
public class SingletonAttribute : Attribute
{
    /// <summary>
    /// A service type that is implemented with the Singleton.
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// An optional method serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool SingletonCondition();
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool SingletonCondition();
    /// </code>
    /// </example>
    public string? ConditionMethod { get; set; }

    /// <summary>
    /// An optional property serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool SingletonCondition { get; }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool SingletonCondition { get; }
    /// </code>
    /// </example>
    public string? ConditionProperty { get; set; }
}