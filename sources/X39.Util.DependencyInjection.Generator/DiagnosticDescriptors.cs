using Microsoft.CodeAnalysis;

namespace X39.Util.DependencyInjection.Generator;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor PrivateConditionMethod = new(
        id: "X39DI001",
        title: "Private condition method",
        messageFormat: "Condition method '{0}' on type '{1}' is private. Change to internal or public for source-generated registration.",
        category: "X39.DependencyInjection",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleDiAttributes = new(
        id: "X39DI002",
        title: "Multiple dependency injection attributes",
        messageFormat: "Type '{0}' has multiple dependency injection attributes",
        category: "X39.DependencyInjection",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidConditionMethodSignature = new(
        id: "X39DI003",
        title: "Invalid condition method signature",
        messageFormat: "Condition method '{0}' on type '{1}' must be static, return bool, and accept zero parameters or a single IConfiguration parameter",
        category: "X39.DependencyInjection",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
