# X39.Util.DependencyInjection
This library adds a simplified workflow for defining implementations for dependencies and adding them to a
dependency container.
It uses Reflection to find all classes in a given assembly which are attributed with either the
`SingletonAttribute`, `TransientAttribute` or `ScopedAttribute` and adds them to the container.
It also allows for `IConfiguration` based selection of active implementations by adding a static
method to the attributed class and adding a `DepependencyInjectionConfigurationAttribute` to the method.

# Semantic Versioning
This library follows the principles of [Semantic Versioning](https://semver.org/).
TThis means that version numbers and the way they change convey meaning about the underlying changes in the library.
For example, if a minor version number changes (e.g., 1.1 to 1.2),
this indicates that new features have been added in a backwards-compatible manner.

# Note on Pre-Net7
This library is fully compatible with pre-.NET 7.0.
However, there is a difference in how the attributes are used, since Net7.0 introduced generic attributes.

The changes pretty much are only in how the attributes have to be written,
eg. `Singleton(typeof(Service), typeof(IService))` instead of `Singleton<Service, IService>`.

This readme will only use the generic attributes. The non-generic attributes are working the same way.

## Quick Start

### Registering the services of an assembly
```csharp
...
// ServiceCollection is the default container of .NET Core
// configuration is an IConfiguration instance
serviceCollection.AddAbstractAttributedServicesOf<TypeInThatAssembly>(configuration);
serviceCollection.AddAbstractAttributedServicesOf(configuration, assemblyReference);
```

```csharp
[Singleton<Service, IService>] // This will make the service findable by the AddAbstractAttributedServicesOf method
public class Service : IService
{
    
}
```

Other than that, you may continue as usual with using dependency injection.

# Attributes
## `SingletonAttribute<TService, [TAbstraction]>`
This attribute marks a class as a singleton. When the `AddAbstractAttributedServicesOf` method is called,
the class will be added to the container as a singleton either as the implementation of the interface.
Internally this means that for a given `Singleton<TService, TAbstraction>`,
the method `AddSingleton<TAbstraction, TService>()` will be called on the service collection.

Alternatively, the attribute can be used without the `TAbstraction` parameter, 
in which case the class will be added as a singleton without an interface
(`AddSingleton<TService>()` for a given `Singleton<TService>`).

*Note that the XML documentation of the attributed class contains samples of how to use the attribute.*

## `TransientAttribute<TService, [TAbstraction]>`
This attribute marks a class as a transient. When the `AddAbstractAttributedServicesOf` method is called,
the class will be added to the container as a transient either as the implementation of the interface.
Internally this means that for a given `Transient<TService, TAbstraction>`,
the method `AddTransient<TAbstraction, TService>()` will be called on the service collection.

Alternatively, the attribute can be used without the `TAbstraction` parameter,
in which case the class will be added as a transient without an interface
(`AddTransient<TService>()` for a given `Transient<TService>`).

*Note that the XML documentation of the attributed class contains samples of how to use the attribute.*

## `ScopedAttribute<TService, [TAbstraction]>`
This attribute marks a class as a scoped
When the `AddAbstractAttributedServicesOf` method is called,
the class will be added to the container as a scoped either as the implementation of the interface.
Internally this means that for a given `Scoped<TService, TAbstraction>`,
the method `AddScoped<TAbstraction, TService>()` will be called on the service collection.

Alternatively, the attribute can be used without the `TAbstraction` parameter,
in which case the class will be added as a scoped without an interface
(`AddScoped<TService>()` for a given `Scoped<TService>`).

*Note that the XML documentation of the attributed class contains samples of how to use the attribute.*

## `DepependencyInjectionConfigurationAttribute`
This attribute marks a static method as a configuration method for the dependency injection.
When the `AddAbstractAttributedServicesOf` method is called.
The method has to be in one of the following forms:
```csharp
     [DependencyInjectionCondition]
     private static bool DICondition()
     {
        ...
     }
```
or
```csharp
     [DependencyInjectionConfiguration]
     private static void DIConfiguration(IConfiguration configuration)
     {
        ...
     }
```

The method will be called on the **initial setup** of the dependency injection container **once**.

If the return value is `false`, the class will not be added to the container.
As can be expected, the value of `true` will add the class to the container.

# Class Constructors
This method will always run any class constructor (if present) before adding the class to the container.

To quickly demonstrate this, consider the following class:
```csharp
[Singleton<IService, Service>]
public class Service : IService
{
    static Service()
    {
        Console.WriteLine("Class constructor called");
    }
}
```

When the `AddAbstractAttributedServicesOf` method is called, the following output will be produced:
```
Class constructor called
```

# Project Notes
This project is part of my personal utility libraries i use in my projects.
The following few paragraphs are meant to give you an overview of the project and how you can contribute to it.

## Building

This project uses GitHub Actions for continuous integration. The workflow is defined in `.github/workflows/main.yml`. It
includes steps for restoring dependencies, building the project, and publishing a NuGet package.

## Test coverage
This project is not yet offering any tests. This is planned for the future, although due to the simplicity of the
project, test coverage currently can be considered as not necessary (testing the project manually is sufficient to me).

If your project is required to have all libraries covered by tests, feel free to submit a pull request with tests.

## Contributing

Contributions are welcome!
Please submit a pull request or create a discussion to discuss any changes you wish to make.

### Code of Conduct

Be excellent to each other.

### Contributors Agreement

First of all, thank you for your interest in contributing to this project!
Please add yourself to the list of contributors in the [CONTRIBUTORS](CONTRIBUTORS.md) file when submitting your
first pull request.
Also, please always add the following to your pull request:

```
By contributing to this project, you agree to the following terms:
- You grant me and any other person who receives a copy of this project the right to use your contribution under the
  terms of the GNU Lesser General Public License v3.0.
- You grant me and any other person who receives a copy of this project the right to relicense your contribution under
  any other license.
- You grant me and any other person who receives a copy of this project the right to change your contribution.
- You waive your right to your contribution and transfer all rights to me and every user of this project.
- You agree that your contribution is free of any third-party rights.
- You agree that your contribution is given without any compensation.
- You agree that I may remove your contribution at any time for any reason.
- You confirm that you have the right to grant the above rights and that you are not violating any third-party rights
  by granting these rights.
- You confirm that your contribution is not subject to any license agreement or other agreement or obligation, which
  conflicts with the above terms.
```

This is necessary to ensure that this project can be licensed under the GNU Lesser General Public License v3.0 and
that a license change is possible in the future if necessary (e.g., to a more permissive license).
It also ensures that I can remove your contribution if necessary (e.g., because it violates third-party rights) and
that I can change your contribution if necessary (e.g., to fix a typo, change implementation details, or improve
performance).
It also shields me and every user of this project from any liability regarding your contribution by deflecting any
potential liability caused by your contribution to you (e.g., if your contribution violates the rights of your
employer).
Feel free to discuss this agreement in the discussions section of this repository, i am open to changes here (as long as
they do not open me or any other user of this project to any liability due to a **malicious contribution**).


## License

This project is licensed under the GNU Lesser General Public License v3.0. See the [LICENSE](LICENSE) file for details.