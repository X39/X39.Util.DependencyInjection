using JetBrains.Annotations;
namespace X39.Util.DependencyInjection.Attributes;

/// <summary>
/// <para>
///     <see cref="Attribute"/> to decorate a <see langword="static"/> method inside of a
///     <see cref="SingletonAttribute"/> or <see cref="TransientAttribute"/> or <see cref="ScopedAttribute"/> decorated
///     <see langword="class"/> with that determines whether or not a class should be used for the dependency injection.
/// </para>
/// <para>
///     If multiple methods in a <see langword="class"/> are decorated with this <see cref="Attribute"/>, all of them
///     must yield <see langword="true"/> for an instance to be created.
/// </para>
/// <para>
///     Valid method signatures always are <see langword="static"/>, always return <see langword="bool"/>'s and always
///     are empty regarding the list of parameters.
/// </para>
/// </summary>
/// <example>
/// <code>
/// [Singleton]
/// public sealed class MySingleton
/// {
///     [DependencyInjectionCondition]
///     private static bool DICondition()
///     {
///         ...
///     }
///     ...
/// }
/// </code>
/// </example>
/// <example>
/// <code>
/// [Singleton]
/// public sealed class MySingleton
/// {
///     [DependencyInjectionCondition]
///     private static bool DICondition(IConfiguration configuration)
///     {
///         ...
///     }
///     ...
/// }
/// </code>
/// </example>
[PublicAPI]
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class DependencyInjectionConditionAttribute : Attribute
{
}