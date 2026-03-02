using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace X39.Util.DependencyInjection.Generator.Tests.TestHelpers;

/// <summary>
/// Custom <see cref="AnalyzerConfigOptionsProvider"/> that provides MSBuild properties to the generator.
/// </summary>
internal sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private readonly TestAnalyzerConfigOptions _globalOptions;

    public TestAnalyzerConfigOptionsProvider(string? className = null, string? namespaceName = null)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string>();
        if (className is not null)
            builder.Add("build_property.X39_DependencyInjection_ClassName", className);
        if (namespaceName is not null)
            builder.Add("build_property.X39_DependencyInjection_Namespace", namespaceName);
        _globalOptions = new TestAnalyzerConfigOptions(builder.ToImmutable());
    }

    public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        => new TestAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty);

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        => new TestAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty);
}