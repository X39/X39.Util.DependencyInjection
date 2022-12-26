# Features
## `SingletonAttribute`
The `SingletonAttribute` is a utility attribute to allow in-code declaration of singleton.

To start using this, call 
```cs
builder.Services.AddMarkedSingletonsFrom(typeof(TypeInTargetAssembly).Assembly)
```
where `TypeInTargetAssembly` is a type, living in the assembly you want to reference.