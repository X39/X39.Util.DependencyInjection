using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection;

/// <summary>
/// Contains the extension functions offered by the X39.Util.DependencyInjection library for the
/// <see cref="ServiceCollection"/> class.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all classes with <see cref="SingletonAttribute"/> set of the <paramref name="assembly"/>
    /// to the <paramref name="services"/> as singleton.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         Given <see cref="SingletonAttribute.ConditionProperty"/> or
    ///         <see cref="SingletonAttribute.ConditionMethod"/> is set,
    ///         it will be evaluated.
    ///         If it resolves to true, it will be added.
    ///         Otherwise it won't.
    ///     </item>
    ///     <item>
    ///         This method will run static constructors on types having <see cref="SingletonAttribute"/> set.
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <exception cref="Exception"></exception>
    internal static void AddAttributedSingletonServicesOf(this IServiceCollection services, Assembly assembly)
    {
#pragma warning disable CS0618
        static bool IsNet6ServiceAttribute(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attribute = type.GetCustomAttribute<SingletonAttribute>();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ServiceType is not null)
            {
                if (!attribute.ServiceType.IsAssignableFrom(type))
                    throw new Exception(
                        $"The type {type.FullName()} is providing the {nameof(SingletonAttribute.ServiceType)} " +
                        $"{attribute.ServiceType.FullName()} but is not implementing that.");
            }

            serviceType = attribute.ServiceType ?? type;
            return true;
        }
#pragma warning restore CS0618
        #if NET7_0_OR_GREATER
        static bool IsSelfContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(SingletonAttribute<>)))
                .OfType<IDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }

        static bool IsContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(SingletonAttribute<,>)))
                .OfType<IAbstractedDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }
        #endif

        static IEnumerable<(Type ServiceType, Type ImplementationType)> GetSingletonTypeTuples(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var implementationType in types)
            {
                // ReSharper disable once InlineOutVariableDeclaration
                Type? serviceType;
                #if NET7_0_OR_GREATER
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType)
                                    && !IsContractedService(implementationType, out serviceType)
                                    && !IsSelfContractedService(implementationType, out serviceType);
                #else
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType);
                #endif
                if (skipCondition)
                    continue;

                yield return (serviceType!, implementationType);
            }
        }

        foreach (var (serviceType, implementingType) in GetSingletonTypeTuples(assembly))
        {
            services.AddSingleton(serviceType, implementingType);
        }
    }
    /// <summary>
    /// Adds all classes with <see cref="TransientAttribute"/> set of the <paramref name="assembly"/>
    /// to the <paramref name="services"/> as transient.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         Given <see cref="TransientAttribute.ConditionProperty"/> or
    ///         <see cref="TransientAttribute.ConditionMethod"/> is set,
    ///         it will be evaluated.
    ///         If it resolves to true, it will be added.
    ///         Otherwise it won't.
    ///     </item>
    ///     <item>
    ///         This method will run static constructors on types having <see cref="TransientAttribute"/> set.
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <exception cref="Exception"></exception>
    internal static void AddAttributedTransientServicesOf(this IServiceCollection services, Assembly assembly)
    {
#pragma warning disable CS0618
        static bool IsNet6ServiceAttribute(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attribute = type.GetCustomAttribute<TransientAttribute>();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ServiceType is not null)
            {
                if (!attribute.ServiceType.IsAssignableFrom(type))
                    throw new Exception(
                        $"The type {type.FullName()} is providing the {nameof(TransientAttribute.ServiceType)} " +
                        $"{attribute.ServiceType.FullName()} but is not implementing that.");
            }

            serviceType = attribute.ServiceType ?? type;
            return true;
        }
#pragma warning restore CS0618
        #if NET7_0_OR_GREATER
        static bool IsSelfContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(TransientAttribute<>)))
                .OfType<IDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }

        static bool IsContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(TransientAttribute<,>)))
                .OfType<IAbstractedDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }
        #endif

        static IEnumerable<(Type ServiceType, Type ImplementationType)> GetTransientTypeTuples(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var implementationType in types)
            {
                // ReSharper disable once InlineOutVariableDeclaration
                Type? serviceType;
                #if NET7_0_OR_GREATER
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType)
                                    && !IsContractedService(implementationType, out serviceType)
                                    && !IsSelfContractedService(implementationType, out serviceType);
                #else
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType);
                #endif
                if (skipCondition)
                    continue;

                yield return (serviceType!, implementationType);
            }
        }

        foreach (var (serviceType, implementingType) in GetTransientTypeTuples(assembly))
        {
            services.AddTransient(serviceType, implementingType);
        }
    }
    /// <summary>
    /// Adds all classes with <see cref="ScopedAttribute"/> set of the <paramref name="assembly"/>
    /// to the <paramref name="services"/> as scoped.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         Given <see cref="ScopedAttribute.ConditionProperty"/> or
    ///         <see cref="ScopedAttribute.ConditionMethod"/> is set,
    ///         it will be evaluated.
    ///         If it resolves to true, it will be added.
    ///         Otherwise it won't.
    ///     </item>
    ///     <item>
    ///         This method will run static constructors on types having <see cref="ScopedAttribute"/> set.
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <exception cref="Exception"></exception>
    internal static void AddAttributedScopedServicesOf(this IServiceCollection services, Assembly assembly)
    {
#pragma warning disable CS0618
        static bool IsNet6ServiceAttribute(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attribute = type.GetCustomAttribute<ScopedAttribute>();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ServiceType is not null)
            {
                if (!attribute.ServiceType.IsAssignableFrom(type))
                    throw new Exception(
                        $"The type {type.FullName()} is providing the {nameof(ScopedAttribute.ServiceType)} " +
                        $"{attribute.ServiceType.FullName()} but is not implementing that.");
            }

            serviceType = attribute.ServiceType ?? type;
            return true;
        }
#pragma warning restore CS0618
        #if NET7_0_OR_GREATER
        static bool IsSelfContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(ScopedAttribute<>)))
                .OfType<IDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }

        static bool IsContractedService(Type type, [NotNullWhen(true)] out Type? serviceType)
        {
            var attributes = type.GetCustomAttributes();
            var attribute = attributes
                .Where((q) => q.GetType().IsGenericType(typeof(ScopedAttribute<,>)))
                .OfType<IAbstractedDependencyInjectionAttribute>()
                .FirstOrDefault();
            if (attribute is null)
            {
                serviceType = null;
                return false;
            }

            if (!type.IsSealed)
                throw new Exception($"The type {type.FullName()} is not sealed.");
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            if (attribute.ConditionMethod is not null)
            {
                var conditionMethods = type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((methodInfo) => methodInfo.GetParameters().Length == 0)
                    .Where((methodInfo) => methodInfo.ReturnType.IsEquivalentTo(typeof(bool)))
                    .ToArray();
                if (conditionMethods.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but does not supply one matching the expected signature.");
                var conditionMethod = conditionMethods.Single();
                var result = conditionMethod.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "method but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (attribute.ConditionProperty is not null)
            {
                var conditionProperties = type
                    .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where((propertyInfo) => propertyInfo.PropertyType.IsEquivalentTo(typeof(bool)))
                    .Where((propertyInfo) => propertyInfo.GetMethod is not null)
                    .Where((propertyInfo) => propertyInfo.SetMethod is null)
                    .ToArray();
                if (conditionProperties.Length != 1)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but does not supply one matching the expected signature.");
                var conditionProperty = conditionProperties.Single();
                var result = conditionProperty.GetMethod!.Invoke(null, Array.Empty<object>());
                if (result is not bool conditionResult)
                    throw new Exception(
                        $"The type {type.FullName()} is marked to have a condition " +
                        "property but the result is not a valid boolean.");
                if (!conditionResult)
                {
                    serviceType = null;
                    return false;
                }
            }

            if (!attribute.ServiceType.IsAssignableFrom(type))
                throw new Exception(
                    $"The type {type.FullName()} is providing the {nameof(IDependencyInjectionAttribute.ServiceType)} " +
                    $"{attribute.ServiceType.FullName()} but is not implementing that.");

            serviceType = attribute.ServiceType;
            return true;
        }
        #endif

        static IEnumerable<(Type ServiceType, Type ImplementationType)> GetScopedTypeTuples(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var implementationType in types)
            {
                // ReSharper disable once InlineOutVariableDeclaration
                Type? serviceType;
                #if NET7_0_OR_GREATER
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType)
                                    && !IsContractedService(implementationType, out serviceType)
                                    && !IsSelfContractedService(implementationType, out serviceType);
                #else
                var skipCondition = !IsNet6ServiceAttribute(implementationType, out serviceType);
                #endif
                if (skipCondition)
                    continue;

                yield return (serviceType!, implementationType);
            }
        }

        foreach (var (serviceType, implementingType) in GetScopedTypeTuples(assembly))
        {
            services.AddScoped(serviceType, implementingType);
        }
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Adds all scoped, transient and singleton dependency injection services which are attributed from the given
    /// <paramref name="assembly"/>.
    /// </summary>
    /// <remarks>This method will run static constructors on the services added by this.</remarks>
    /// <seealso cref="SingletonAttribute{TService}"/>
    /// <seealso cref="SingletonAttribute{TService,TAbstraction}"/>
    /// <seealso cref="TransientAttribute{TService}"/>
    /// <seealso cref="TransientAttribute{TService,TAbstraction}"/>
    /// <seealso cref="ScopedAttribute{TService}"/>
    /// <seealso cref="ScopedAttribute{TService,TAbstraction}"/>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">
    /// The <see cref="Assembly"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#else
    /// <summary>
    /// Adds all scoped, transient and singleton dependency injection services which are attributed from the given
    /// <paramref name="assembly"/>.
    /// </summary>
    /// <remarks>This method will run static constructors on the services added by this.</remarks>
    /// <seealso cref="SingletonAttribute"/>
    /// <seealso cref="TransientAttribute"/>
    /// <seealso cref="ScopedAttribute"/>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">
    /// The <see cref="Assembly"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#endif
    public static IServiceCollection AddAttributedServicesOf(
        this IServiceCollection serviceCollection,
        Assembly assembly)
    {
        serviceCollection.AddAttributedSingletonServicesOf(assembly);
        serviceCollection.AddAttributedTransientServicesOf(assembly);
        serviceCollection.AddAttributedScopedServicesOf(assembly);
        return serviceCollection;
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Adds all scoped, transient and singleton dependency injection services which are attributed from the given
    /// <paramref name="appDomain"/>.
    /// </summary>
    /// <remarks>This method will run static constructors on the services added by this.</remarks>
    /// <seealso cref="SingletonAttribute{TService}"/>
    /// <seealso cref="SingletonAttribute{TService,TAbstraction}"/>
    /// <seealso cref="TransientAttribute{TService}"/>
    /// <seealso cref="TransientAttribute{TService,TAbstraction}"/>
    /// <seealso cref="ScopedAttribute{TService}"/>
    /// <seealso cref="ScopedAttribute{TService,TAbstraction}"/>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="appDomain">
    /// The <see cref="AppDomain"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#else
    /// <summary>
    /// Adds all scoped, transient and singleton dependency injection services which are attributed from the given
    /// <paramref name="appDomain"/>.
    /// </summary>
    /// <remarks>This method will run static constructors on the services added by this.</remarks>
    /// <seealso cref="SingletonAttribute"/>
    /// <seealso cref="TransientAttribute"/>
    /// <seealso cref="ScopedAttribute"/>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="appDomain">
    /// The <see cref="AppDomain"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#endif
    public static IServiceCollection AddAttributedServicesOf(
        this IServiceCollection serviceCollection,
        AppDomain appDomain)
    {
        foreach (var assembly in appDomain.GetAssemblies())
        {
            serviceCollection.AddAttributedServicesOf(assembly);
        }
        return serviceCollection;
    }
}