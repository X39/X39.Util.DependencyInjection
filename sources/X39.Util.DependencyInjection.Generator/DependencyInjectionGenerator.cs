using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace X39.Util.DependencyInjection.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class DependencyInjectionGenerator : IIncrementalGenerator
{
    private static readonly string[] KnownAttributeNames =
    {
        "SingletonAttribute",
        "TransientAttribute",
        "ScopedAttribute",
    };

    private static readonly string[] KnownAttributeFullNames =
    {
        "X39.Util.DependencyInjection.Attributes.SingletonAttribute",
        "X39.Util.DependencyInjection.Attributes.TransientAttribute",
        "X39.Util.DependencyInjection.Attributes.ScopedAttribute",
    };

    private const string ConditionAttributeFullName =
        "X39.Util.DependencyInjection.Attributes.DependencyInjectionConditionAttribute";

    private const string IConfigurationFullName =
        "Microsoft.Extensions.Configuration.IConfiguration";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Syntax filter — find class declarations with at least one attribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: static (ctx, ct) => TransformCandidate(ctx, ct))
            .Where(static result => result.Registration is not null || !result.Diagnostics.IsEmpty);

        // Step 2: Combine with MSBuild options
        var optionsProvider = context.AnalyzerConfigOptionsProvider;

        var combined = classDeclarations.Collect().Combine(optionsProvider);

        // Step 3: Output
        context.RegisterSourceOutput(combined, static (spc, pair) =>
        {
            var (candidates, options) = pair;
            Execute(spc, candidates, options);
        });
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        // Fast filter: only class declarations with at least one attribute
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        if (classDecl.AttributeLists.Count == 0)
            return false;

        // Quick name check — look for attribute names that might match
        foreach (var attributeList in classDecl.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var name = ExtractAttributeName(attribute);
                foreach (var known in KnownAttributeNames)
                {
                    // Match "Singleton", "SingletonAttribute", "Singleton<...>", etc.
                    if (name == known || name + "Attribute" == known
                        || known.StartsWith(name))
                        return true;
                }
            }
        }

        return false;
    }

    private static string ExtractAttributeName(AttributeSyntax attribute)
    {
        // Handle generic attributes: Singleton<T> -> GenericNameSyntax
        // Handle qualified names: X39...Singleton -> QualifiedNameSyntax
        var nameSyntax = attribute.Name;

        switch (nameSyntax)
        {
            case GenericNameSyntax generic:
                return generic.Identifier.Text;
            case IdentifierNameSyntax identifier:
                return identifier.Identifier.Text;
            case QualifiedNameSyntax qualified:
                // Take the rightmost name
                return qualified.Right switch
                {
                    GenericNameSyntax g => g.Identifier.Text,
                    _ => qualified.Right.Identifier.Text,
                };
            case AliasQualifiedNameSyntax alias:
                return alias.Name.Identifier.Text;
            default:
                return nameSyntax.ToString();
        }
    }

    private readonly struct CandidateResult
    {
        public ServiceRegistration? Registration { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public Location? DiagnosticLocation { get; }

        public CandidateResult(ServiceRegistration? registration, ImmutableArray<Diagnostic> diagnostics, Location? diagnosticLocation = null)
        {
            Registration = registration;
            Diagnostics = diagnostics;
            DiagnosticLocation = diagnosticLocation;
        }
    }

    private static CandidateResult TransformCandidate(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl, ct);
        if (classSymbol is null)
            return new CandidateResult(null, ImmutableArray<Diagnostic>.Empty);

        // Find all DI attributes on this class
        var diAttributes = new List<(AttributeData Attr, string Lifetime)>();

        foreach (var attr in classSymbol.GetAttributes())
        {
            var attrClass = attr.AttributeClass;
            if (attrClass is null)
                continue;

            var lifetime = GetLifetime(attrClass);
            if (lifetime is not null)
            {
                diAttributes.Add((attr, lifetime));
            }
        }

        if (diAttributes.Count == 0)
            return new CandidateResult(null, ImmutableArray<Diagnostic>.Empty);

        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

        // Check for multiple DI attributes
        if (diAttributes.Count > 1)
        {
            diagnostics.Add(Diagnostic.Create(
                DiagnosticDescriptors.MultipleDiAttributes,
                classDecl.Identifier.GetLocation(),
                classSymbol.ToDisplayString()));
            return new CandidateResult(null, diagnostics.ToImmutable());
        }

        var (diAttr, diLifetime) = diAttributes[0];
        var attrClassSymbol = diAttr.AttributeClass!;

        // Determine service type and whether there's an abstraction
        string fullyQualifiedClassName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string fullyQualifiedServiceType;
        bool hasAbstraction;

        // Remove "global::" prefix for cleaner generated code
        fullyQualifiedClassName = RemoveGlobalPrefix(fullyQualifiedClassName);

        if (attrClassSymbol.IsGenericType)
        {
            var typeArgs = attrClassSymbol.TypeArguments;
            if (typeArgs.Length == 2)
            {
                // Generic 2-param: Singleton<TService, TAbstraction>
                // TService = typeArgs[0] (the implementation), TAbstraction = typeArgs[1] (the service interface)
                fullyQualifiedServiceType = RemoveGlobalPrefix(typeArgs[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                hasAbstraction = true;
            }
            else if (typeArgs.Length == 1)
            {
                // Generic 1-param: Singleton<TService>
                fullyQualifiedServiceType = RemoveGlobalPrefix(typeArgs[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                hasAbstraction = false;
            }
            else
            {
                fullyQualifiedServiceType = fullyQualifiedClassName;
                hasAbstraction = false;
            }
        }
        else
        {
            // Non-generic attribute: Singleton(typeof(IService), typeof(Service))
            var ctorArgs = diAttr.ConstructorArguments;
            if (ctorArgs.Length == 2)
            {
                // 2 args: (serviceType, actualType)
                var serviceTypeSymbol = ctorArgs[0].Value as INamedTypeSymbol;
                var actualTypeSymbol = ctorArgs[1].Value as INamedTypeSymbol;
                fullyQualifiedServiceType = serviceTypeSymbol is not null
                    ? RemoveGlobalPrefix(serviceTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    : fullyQualifiedClassName;
                hasAbstraction = serviceTypeSymbol is not null && actualTypeSymbol is not null
                    && !SymbolEqualityComparer.Default.Equals(serviceTypeSymbol, actualTypeSymbol);
            }
            else if (ctorArgs.Length == 1)
            {
                // 1 arg: (serviceType)
                var serviceTypeSymbol = ctorArgs[0].Value as INamedTypeSymbol;
                fullyQualifiedServiceType = serviceTypeSymbol is not null
                    ? RemoveGlobalPrefix(serviceTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    : fullyQualifiedClassName;
                // Single arg means ServiceType == ActualType — no abstraction
                hasAbstraction = false;
            }
            else
            {
                fullyQualifiedServiceType = fullyQualifiedClassName;
                hasAbstraction = false;
            }
        }

        // Find condition methods
        var conditionMethods = new List<ConditionMethod>();
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IMethodSymbol method)
                continue;

            var hasConditionAttr = method.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == ConditionAttributeFullName);

            if (!hasConditionAttr)
                continue;

            // Validate signature: must be static, return bool, 0 or 1 IConfiguration param
            bool isValid = method.IsStatic
                && method.ReturnType.SpecialType == SpecialType.System_Boolean;

            bool hasConfigParam = false;

            if (isValid)
            {
                if (method.Parameters.Length == 0)
                {
                    // OK — no params
                }
                else if (method.Parameters.Length == 1
                    && method.Parameters[0].Type.ToDisplayString() == IConfigurationFullName)
                {
                    hasConfigParam = true;
                }
                else
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                diagnostics.Add(Diagnostic.Create(
                    DiagnosticDescriptors.InvalidConditionMethodSignature,
                    method.Locations.FirstOrDefault() ?? classDecl.Identifier.GetLocation(),
                    method.Name,
                    classSymbol.ToDisplayString()));
                return new CandidateResult(null, diagnostics.ToImmutable());
            }

            // Check accessibility
            bool isAccessible = method.DeclaredAccessibility != Accessibility.Private;

            if (!isAccessible)
            {
                diagnostics.Add(Diagnostic.Create(
                    DiagnosticDescriptors.PrivateConditionMethod,
                    method.Locations.FirstOrDefault() ?? classDecl.Identifier.GetLocation(),
                    method.Name,
                    classSymbol.ToDisplayString()));
            }

            conditionMethods.Add(new ConditionMethod(method.Name, hasConfigParam, isAccessible));
        }

        // If we have any inaccessible condition methods, still report diagnostics but skip this registration
        bool hasInaccessible = conditionMethods.Any(m => !m.IsAccessible);
        if (hasInaccessible)
        {
            return new CandidateResult(null, diagnostics.ToImmutable());
        }

        var registration = new ServiceRegistration(
            fullyQualifiedClassName,
            fullyQualifiedServiceType,
            hasAbstraction,
            diLifetime,
            new EquatableArray<ConditionMethod>(conditionMethods));

        return new CandidateResult(registration, diagnostics.ToImmutable());
    }

    private static string? GetLifetime(INamedTypeSymbol attrClass)
    {
        // Check the original definition (unbound generic) for generic attributes
        var originalDef = attrClass.OriginalDefinition;
        var fullName = originalDef.ToDisplayString();

        // Strip generic arity for matching: SingletonAttribute<TService> -> X39...SingletonAttribute
        // The OriginalDefinition will have type parameters like SingletonAttribute<TService>
        // so we compare against known patterns

        foreach (var knownName in KnownAttributeFullNames)
        {
            // Exact match for non-generic
            if (fullName == knownName)
                return ExtractLifetimeFromName(knownName);

            // Generic match: fullName starts with the known base + "<"
            if (fullName.StartsWith(knownName + "<"))
                return ExtractLifetimeFromName(knownName);
        }

        // Also check base types — the non-generic SingletonAttribute is a class, not sealed
        // and the generic versions are separate sealed classes that don't inherit from it.
        // Check if this inherits from a known attribute
        var baseType = attrClass.BaseType;
        while (baseType is not null)
        {
            var baseName = baseType.OriginalDefinition.ToDisplayString();
            foreach (var knownName in KnownAttributeFullNames)
            {
                if (baseName == knownName)
                    return ExtractLifetimeFromName(knownName);
            }
            baseType = baseType.BaseType;
        }

        return null;
    }

    private static string ExtractLifetimeFromName(string fullName)
    {
        if (fullName.Contains("Singleton")) return "Singleton";
        if (fullName.Contains("Transient")) return "Transient";
        if (fullName.Contains("Scoped")) return "Scoped";
        return "Singleton";
    }

    private static string RemoveGlobalPrefix(string name)
    {
        const string prefix = "global::";
        return name.StartsWith(prefix) ? name.Substring(prefix.Length) : name;
    }

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<CandidateResult> candidates,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        // Report all diagnostics
        foreach (var candidate in candidates)
        {
            foreach (var diagnostic in candidate.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        // Gather registrations
        var registrations = candidates
            .Where(c => c.Registration is not null)
            .Select(c => c.Registration!)
            .ToList();

        // Read MSBuild properties
        optionsProvider.GlobalOptions.TryGetValue(
            "build_property.X39_DependencyInjection_ClassName", out var className);
        optionsProvider.GlobalOptions.TryGetValue(
            "build_property.X39_DependencyInjection_Namespace", out var namespaceName);

        if (string.IsNullOrWhiteSpace(className))
            className = "Dependencies";
        if (string.IsNullOrWhiteSpace(namespaceName))
            namespaceName = "X39.Util.DependencyInjection";

        var methodName = "Add" + className;

        // Generate source
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
        sb.AppendLine($"    /// <summary>");
        sb.AppendLine($"    /// Auto-generated dependency injection registration.");
        sb.AppendLine($"    /// </summary>");
        sb.AppendLine($"    public static class {className}");
        sb.AppendLine("    {");
        sb.AppendLine($"        /// <summary>");
        sb.AppendLine($"        /// Registers all attributed services discovered at compile time.");
        sb.AppendLine($"        /// </summary>");
        sb.AppendLine($"        public static IServiceCollection {methodName}(");
        sb.AppendLine($"            this IServiceCollection services,");
        sb.AppendLine($"            IConfiguration configuration)");
        sb.AppendLine("        {");

        foreach (var reg in registrations)
        {
            sb.AppendLine();
            sb.AppendLine($"            RuntimeHelpers.RunClassConstructor(typeof({reg.FullyQualifiedClassName}).TypeHandle);");

            var registrationCall = GenerateRegistrationCall(reg);

            if (reg.ConditionMethods.Length > 0)
            {
                // Build condition expression
                var conditions = new List<string>();
                foreach (var cm in reg.ConditionMethods)
                {
                    if (cm.HasConfigurationParameter)
                        conditions.Add($"{reg.FullyQualifiedClassName}.{cm.Name}(configuration)");
                    else
                        conditions.Add($"{reg.FullyQualifiedClassName}.{cm.Name}()");
                }

                var conditionExpr = string.Join(" && ", conditions);
                sb.AppendLine($"            if ({conditionExpr})");
                sb.AppendLine("            {");
                sb.AppendLine($"                {registrationCall}");
                sb.AppendLine("            }");
            }
            else
            {
                sb.AppendLine($"            {registrationCall}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("            return services;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource($"{className}.g.cs", sb.ToString());
    }

    private static string GenerateRegistrationCall(ServiceRegistration reg)
    {
        if (reg.HasAbstraction)
        {
            return $"services.Add{reg.Lifetime}<{reg.FullyQualifiedServiceType}, {reg.FullyQualifiedClassName}>();";
        }
        else
        {
            return $"services.Add{reg.Lifetime}<{reg.FullyQualifiedClassName}>();";
        }
    }
}
