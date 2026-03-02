using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using X39.Util.DependencyInjection.Attributes;

namespace X39.Util.DependencyInjection.Generator.Tests.TestHelpers;

internal static class GeneratorTestHelper
{
    /// <summary>
    /// Runs the generator with the given source and returns the generated output for the specified hint name.
    /// </summary>
    public static (string GeneratedSource, ImmutableArray<Diagnostic> Diagnostics) RunGenerator(
        string source,
        string hintName = "Dependencies.g.cs",
        string? className = null,
        string? namespaceName = null)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SingletonAttribute<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Configuration.IConfiguration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location),
        };

        // Add runtime assemblies needed for compilation
        var runtimeDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(runtimeDir, "System.Runtime.dll")));

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new DependencyInjectionGenerator();
        var optionsProvider = new TestAnalyzerConfigOptionsProvider(className, namespaceName);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new ISourceGenerator[] { generator.AsSourceGenerator() },
            optionsProvider: optionsProvider);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);

        var runResult = driver.GetRunResult();
        var generatedSource = runResult.Results
            .SelectMany(r => r.GeneratedSources)
            .FirstOrDefault(s => s.HintName == hintName);

        return (generatedSource.SourceText?.ToString() ?? string.Empty, diagnostics);
    }

    /// <summary>
    /// Verifies the generator produces exactly the expected source code.
    /// </summary>
    public static void VerifyGeneratedCode(
        string source,
        string expectedOutput,
        string hintName = "Dependencies.g.cs",
        string? className = null,
        string? namespaceName = null)
    {
        var (generatedSource, _) = RunGenerator(source, hintName, className, namespaceName);
        var normalizedExpected = expectedOutput.Replace("\r\n", "\n");
        var normalizedActual = generatedSource.Replace("\r\n", "\n");
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    /// <summary>
    /// Runs the generator and returns only the diagnostics (for diagnostic tests).
    /// </summary>
    public static ImmutableArray<Diagnostic> GetDiagnostics(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SingletonAttribute<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.Configuration.IConfiguration).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location),
        };

        var runtimeDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        references.Add(MetadataReference.CreateFromFile(System.IO.Path.Combine(runtimeDir, "System.Runtime.dll")));

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new DependencyInjectionGenerator();
        var optionsProvider = new TestAnalyzerConfigOptionsProvider();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new ISourceGenerator[] { generator.AsSourceGenerator() },
            optionsProvider: optionsProvider);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        var runResult = driver.GetRunResult();
        // Return generator-reported diagnostics, not compilation diagnostics
        return runResult.Results
            .SelectMany(r => r.Diagnostics)
            .ToImmutableArray();
    }
}
