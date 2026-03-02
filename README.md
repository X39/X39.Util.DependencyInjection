# X39.Util.DependencyInjection

Attribute-based service registration for `Microsoft.Extensions.DependencyInjection`.

[![NuGet](https://img.shields.io/nuget/v/X39.Util.DependencyInjection)](https://www.nuget.org/packages/X39.Util.DependencyInjection)

## Installation

```shell
dotnet add package X39.Util.DependencyInjection
```

Or add directly to your `.csproj`:

```xml
<PackageReference Include="X39.Util.DependencyInjection" Version="*" />
```

## Quick Start

Decorate your service class with a lifetime attribute and call `AddAttributedServicesOf` during startup:

```csharp
using X39.Util.DependencyInjection;
using X39.Util.DependencyInjection.Attributes;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAttributedServicesOf(context.Configuration, typeof(Program).Assembly);
    })
    .Build();

host.Run();
```

```csharp
public interface IMyService
{
    void DoWork();
}

[Singleton<MyService, IMyService>]
public class MyService : IMyService
{
    public void DoWork() { /* ... */ }
}
```

## API Reference

### Registration Methods

All registration methods are extension methods on `IServiceCollection`.

#### `AddAttributedServicesOf(IConfiguration, Assembly)`

Scans the given assembly for classes decorated with lifetime attributes and registers them.

```csharp
services.AddAttributedServicesOf(configuration, typeof(Program).Assembly);
```

#### `AddAttributedServicesFromAssemblyOf<T>(IConfiguration)`

Convenience overload that scans the assembly containing type `T`.

```csharp
services.AddAttributedServicesFromAssemblyOf<Program>(configuration);
```

#### `AddAttributedServicesOf(IConfiguration, AppDomain)`

Scans all assemblies loaded in the given `AppDomain`.

```csharp
services.AddAttributedServicesOf(configuration, AppDomain.CurrentDomain);
```

### Lifetime Attributes

Each attribute corresponds to a standard DI lifetime. The generic forms require .NET 7+.

| Attribute | Lifetime | Equivalent call |
|-----------|----------|-----------------|
| `[Singleton<TService>]` | Singleton | `AddSingleton<TService>()` |
| `[Singleton<TService, TAbstraction>]` | Singleton | `AddSingleton<TAbstraction, TService>()` |
| `[Transient<TService>]` | Transient | `AddTransient<TService>()` |
| `[Transient<TService, TAbstraction>]` | Transient | `AddTransient<TAbstraction, TService>()` |
| `[Scoped<TService>]` | Scoped | `AddScoped<TService>()` |
| `[Scoped<TService, TAbstraction>]` | Scoped | `AddScoped<TAbstraction, TService>()` |

In the two-type-parameter form, `TService` is the implementation class and `TAbstraction` is the
interface or base class (`TService : TAbstraction`).

**Pre-.NET 7:** Non-generic versions are available using `typeof(...)`:

```csharp
// Without abstraction (registers as itself)
[Singleton(typeof(MyService))]

// With abstraction (note: parameter order is serviceType, actualType)
[Singleton(typeof(IMyService), typeof(MyService))]
```

### Conditional Registration

Use `[DependencyInjectionCondition]` on a static method to control whether a service is registered.
The method must be `static`, return `bool`, and accept either no parameters or a single
`IConfiguration` parameter.

```csharp
public interface IMyService
{
    bool SomeFunc();
}

[Singleton<DebugService, IMyService>]
public class DebugService : IMyService
{
    [DependencyInjectionCondition]
    private static bool Condition()
    {
        #if DEBUG
        return true;
        #else
        return false;
        #endif
    }

    public bool SomeFunc() => true;
}

[Singleton<ReleaseService, IMyService>]
public class ReleaseService : IMyService
{
    [DependencyInjectionCondition]
    private static bool Condition()
    {
        #if DEBUG
        return false;
        #else
        return true;
        #endif
    }

    public bool SomeFunc() => true;
}
```

A condition method can also accept `IConfiguration` to make decisions based on app configuration:

```csharp
[DependencyInjectionCondition]
private static bool IsEnabled(IConfiguration configuration)
{
    return configuration.GetValue<bool>("Features:MyService");
}
```

If a class has multiple condition methods, **all** must return `true` for the service to be
registered (AND logic).

## Behavior Notes

- **Static constructors** are executed during registration (before condition methods are evaluated).
- Only **one** lifetime attribute is allowed per class. Applying more than one throws
  `MultipleDependencyInjectionAttributesPresentException`.
- Lifetime attributes are **not inherited** (`Inherited = false`).

## Exceptions

All exceptions derive from `DependencyInjectionException`.

| Exception | Thrown when |
|-----------|------------|
| `ActualTypeIsNotMatchingDecoratedTypeException` | The `TService` type parameter does not match the class the attribute is applied to. |
| `ConditionMethodHasInvalidSignatureException` | A `[DependencyInjectionCondition]` method is not static, does not return `bool`, or has unsupported parameters. |
| `MultipleDependencyInjectionAttributesPresentException` | A class has more than one lifetime attribute (`[Singleton]`, `[Transient]`, `[Scoped]`). |
| `ServiceTypeIsNotImplementingDecoratedTypeException` | The decorated class does not implement the `TAbstraction` type. |

## Semantic Versioning

This library follows the principles of [Semantic Versioning](https://semver.org/).

## Contributing

Contributions are welcome!
Please submit a pull request or create a discussion to discuss any changes you wish to make.

### Code of Conduct

Be excellent to each other.

### Contributor License Agreement

By submitting a contribution (pull request, patch, or any other form) to this project, you agree
to the following terms:

1. **License Grant.** You grant the project maintainer ("Maintainer") and all recipients of the
   software a perpetual, worldwide, non-exclusive, royalty-free, irrevocable license to use,
   reproduce, modify, distribute, sublicense, and otherwise exploit your contribution under the
   terms of the GNU Lesser General Public License v3.0 (LGPL-3.0-only). You additionally grant
   the Maintainer the right to relicense your contribution under any other open-source or
   proprietary license at the Maintainer's sole discretion.

2. **Originality.** You represent that your contribution is your original work, or that you have
   sufficient rights to grant the licenses above. If your contribution includes third-party
   material, you represent that its license is compatible with the LGPL-3.0-only and permits the
   grants made herein.

3. **No Conflicting Obligations.** You represent that your contribution is not subject to any
   agreement, obligation, or encumbrance (including but not limited to employment agreements or
   prior license grants) that would conflict with or restrict the rights granted under this
   agreement.

4. **No Compensation.** Your contribution is made voluntarily and without expectation of
   compensation, unless separately agreed in writing.

5. **Right to Remove.** The Maintainer may remove, modify, or replace your contribution at any
   time, for any reason, without notice or obligation to you.

6. **Liability.** To the maximum extent permitted by applicable law, your contribution is provided
   "as is", without warranty of any kind. You shall be solely liable for any damage arising from
   the inclusion of your contribution to the extent such damage is caused by a defect, rights
   violation, or other issue originating in your contribution.

7. **Governing Law.** This agreement is governed by the laws of the Federal Republic of Germany
   (Bundesrepublik Deutschland), in particular the German Civil Code (BGB), without regard to
   its conflict-of-laws provisions. For contributors outside Germany, this choice of law applies
   to the extent permitted by the contributor's local jurisdiction.

Please add yourself to the [CONTRIBUTORS](CONTRIBUTORS.md) file when submitting your first pull
request, and include the following statement in your pull request description:

> I have read and agree to the Contributor License Agreement in this project's README.

## License

This project is licensed under the GNU Lesser General Public License v3.0.
See the [LICENSE](LICENSE) file for details.
