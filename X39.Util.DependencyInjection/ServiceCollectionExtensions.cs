using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X39.Util.DependencyInjection.Attributes;
using X39.Util.DependencyInjection.Exceptions;

namespace X39.Util.DependencyInjection;

/// <summary>
/// Contains the extension functions offered by the X39.Util.DependencyInjection library for the
/// <see cref="ServiceCollection"/> class.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    internal static void AddAbstractAttributedServicesOf(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!TryGetDependencyInjectionAttribute(type, out var dependencyInjectionAttribute))
                continue;
            if (!TryMaterializeService(configuration, type, dependencyInjectionAttribute, out var serviceType,
                    out var actualType))
                continue;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (dependencyInjectionAttribute.Kind)
            {
                case EDependencyInjectionKind.Singleton:
                    services.AddSingleton(serviceType, actualType);
                    break;
                case EDependencyInjectionKind.Scoped:
                    services.AddScoped(serviceType, actualType);
                    break;
                case EDependencyInjectionKind.Transient:
                    services.AddTransient(serviceType, actualType);
                    break;
            }
        }
    }

    private static bool TryMaterializeService(
        IConfiguration configuration,
        Type type,
        IDependencyInjectionAttribute dependencyInjectionAttribute,
        out Type serviceType,
        out Type actualType)
    {
        actualType = dependencyInjectionAttribute is IAbstractedDependencyInjectionAttribute abstracted
            ? abstracted.ActualType
            : dependencyInjectionAttribute.ServiceType;
        serviceType = dependencyInjectionAttribute.ServiceType;

        if (!actualType.IsEquivalentTo(type))
            throw new ActualTypeIsNotMatchingDecoratedTypeException(type, actualType);

        if (!serviceType.IsAssignableFrom(type))
            throw new ServiceTypeIsNotImplementingDecoratedTypeException(type, serviceType);

        var conditionMethodInfos = actualType
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic)
            .Where((mInfo) => mInfo.GetCustomAttribute<DependencyInjectionConditionAttribute>() is not null)
            .ToArray();
        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        var materialize = true;
        object[]? data = null;
        foreach (var methodInfo in conditionMethodInfos)
        {
            if (methodInfo.IsStatic is false)
                throw new ConditionMethodHasInvalidSignatureException(type, methodInfo);
            if (methodInfo.ReturnType.IsEquivalentTo(typeof(bool)) is false)
                throw new ConditionMethodHasInvalidSignatureException(type, methodInfo);
            var methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length > 0)
            {
                if (methodParameters.Length is not 0 and not 1)
                    throw new ConditionMethodHasInvalidSignatureException(type, methodInfo);
                if (methodParameters.Any(q => q.ParameterType.IsEquivalentTo(typeof(IConfiguration))))
                    throw new ConditionMethodHasInvalidSignatureException(type, methodInfo);
                materialize = materialize && (bool) methodInfo.Invoke(null, data ??= new object[] {configuration})!;
                if (materialize is false)
                    break;
            }
            else
            {
                materialize = materialize && (bool) methodInfo.Invoke(null, Array.Empty<object>())!;
                if (materialize is false)
                    break;
            }
        }

        return materialize;
    }

    private static bool TryGetDependencyInjectionAttribute(
        Type type,
        [NotNullWhen(true)] out IDependencyInjectionAttribute? dependencyInjectionAttribute)
    {
        var attributes = type.GetCustomAttributes();
        var dependencyInjectionAttributes = attributes.OfType<IDependencyInjectionAttribute>().ToArray();
        switch (dependencyInjectionAttributes.Length)
        {
            case > 1:
                throw new MultipleDependencyInjectionAttributesPresentException(
                    type,
                    dependencyInjectionAttributes.OfType<Attribute>().ToArray());
            case 0:
                dependencyInjectionAttribute = null;
                return false;
            default:
                dependencyInjectionAttribute = dependencyInjectionAttributes.First();
                return true;
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
    /// <param name="configuration">The <see cref="IConfiguration"/> provider of the application.</param>
    /// <param name="assembly">
    /// The <see cref="Assembly"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#endif
    public static IServiceCollection AddAttributedServicesOf(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Assembly assembly)
    {
        AddAbstractAttributedServicesOf(serviceCollection, configuration, assembly);
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
    /// <param name="configuration">The <see cref="IConfiguration"/> provider of the application.</param>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="appDomain">
    /// The <see cref="AppDomain"/> to scan the <see cref="Type"/>'s for <see cref="Attribute"/>'s.
    /// </param>
    /// <returns>The provide <paramref name="serviceCollection"/> to allow method chaining.</returns>
#endif
    public static IServiceCollection AddAttributedServicesOf(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        AppDomain appDomain)
    {
        foreach (var assembly in appDomain.GetAssemblies())
        {
            serviceCollection.AddAttributedServicesOf(configuration, assembly);
        }

        return serviceCollection;
    }
}