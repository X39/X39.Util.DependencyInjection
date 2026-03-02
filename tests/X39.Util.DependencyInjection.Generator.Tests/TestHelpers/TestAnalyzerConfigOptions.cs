using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace X39.Util.DependencyInjection.Generator.Tests.TestHelpers;

/// <summary>
/// Custom <see cref="AnalyzerConfigOptions"/> that returns values from a dictionary.
/// </summary>
internal sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    private readonly ImmutableDictionary<string, string> _options;

    public TestAnalyzerConfigOptions(ImmutableDictionary<string, string> options)
    {
        _options = options;
    }

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        => _options.TryGetValue(key, out value);
}