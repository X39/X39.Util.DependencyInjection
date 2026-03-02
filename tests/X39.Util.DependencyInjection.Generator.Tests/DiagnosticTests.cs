using X39.Util.DependencyInjection.Generator.Tests.TestHelpers;

namespace X39.Util.DependencyInjection.Generator.Tests;

public class DiagnosticTests
{
    [Fact]
    public void PrivateConditionMethod_ReportsX39DI001()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            namespace TestNamespace
            {
                [Singleton<MyService>]
                public class MyService
                {
                    [DependencyInjectionCondition]
                    private static bool IsEnabled() => true;
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);
        var diagnostic = Assert.Single(diagnostics, d => d.Id == "X39DI001");
        Assert.Contains("IsEnabled", diagnostic.GetMessage());
    }

    [Fact]
    public void MultipleDiAttributes_ReportsX39DI002()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            namespace TestNamespace
            {
                [Singleton<MyService>]
                [Transient<MyService>]
                public class MyService { }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);
        var diagnostic = Assert.Single(diagnostics, d => d.Id == "X39DI002");
        Assert.Contains("TestNamespace.MyService", diagnostic.GetMessage());
    }

    [Fact]
    public void ConditionMethodWrongReturnType_ReportsX39DI003()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            namespace TestNamespace
            {
                [Singleton<MyService>]
                public class MyService
                {
                    [DependencyInjectionCondition]
                    public static int IsEnabled() => 1;
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);
        var diagnostic = Assert.Single(diagnostics, d => d.Id == "X39DI003");
        Assert.Contains("IsEnabled", diagnostic.GetMessage());
    }

    [Fact]
    public void ConditionMethodWrongParam_ReportsX39DI003()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            namespace TestNamespace
            {
                [Singleton<MyService>]
                public class MyService
                {
                    [DependencyInjectionCondition]
                    public static bool IsEnabled(string name) => true;
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);
        var diagnostic = Assert.Single(diagnostics, d => d.Id == "X39DI003");
        Assert.Contains("IsEnabled", diagnostic.GetMessage());
    }

    [Fact]
    public void ConditionMethodTooManyParams_ReportsX39DI003()
    {
        var source = """
            using Microsoft.Extensions.Configuration;
            using X39.Util.DependencyInjection.Attributes;

            namespace TestNamespace
            {
                [Singleton<MyService>]
                public class MyService
                {
                    [DependencyInjectionCondition]
                    public static bool IsEnabled(IConfiguration config, string extra) => true;
                }
            }
            """;

        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);
        var diagnostic = Assert.Single(diagnostics, d => d.Id == "X39DI003");
        Assert.Contains("IsEnabled", diagnostic.GetMessage());
    }
}
