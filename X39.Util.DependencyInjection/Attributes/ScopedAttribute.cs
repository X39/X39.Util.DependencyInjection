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
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;(ConditionProperty = nameof(IsLoaded))]
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
///     [Scoped&lt;MyConditionalService, IMyConditionalService&gt;(ConditionProperty = nameof(IsLoaded))]
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
public sealed class ScopedAttribute<TService, TAbstraction> : Attribute where TService : TAbstraction
{
    /// <summary>
    /// The service type that is implemented with the Scoped.
    /// </summary>
    public Type? ServiceType { get; } = typeof(TAbstraction);

    /// <summary>
    /// An optional method serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool ScopedCondition();
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool ScopedCondition();
    /// </code>
    /// </example>
    public string? ConditionMethod { get; set; }

    /// <summary>
    /// An optional property serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool ScopedCondition { get; }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool ScopedCondition { get; }
    /// </code>
    /// </example>
    public string? ConditionProperty { get; set; }
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
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class ScopedAttribute<TService> : Attribute
{
    /// <summary>
    /// The service type that is implemented with the Scoped.
    /// </summary>
    public Type? ServiceType { get; } = typeof(TService);
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
///     [Scoped(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
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
///     [Scoped(ServiceType = typeof(IMyConditionalService), ConditionProperty = nameof(IsLoaded))]
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
public class ScopedAttribute : Attribute
{
    /// <summary>
    /// An optional service type that is implemented with the Scoped.
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// An optional method serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool ScopedCondition();
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool ScopedCondition();
    /// </code>
    /// </example>
    public string? ConditionMethod { get; set; }

    /// <summary>
    /// An optional property serving as condition for whether the type should be appended or not.
    /// Must live in the implementing type.
    /// </summary>
    /// <example>
    /// <code>
    ///     public static bool ScopedCondition { get; }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    ///     private static bool ScopedCondition { get; }
    /// </code>
    /// </example>
    public string? ConditionProperty { get; set; }
}