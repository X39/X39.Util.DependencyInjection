using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using X39.Util.DependencyInjection.Exceptions;
using X39.Util.DependencyInjection.Tests.TestHelpers;

namespace X39.Util.DependencyInjection.Tests;

public class ExceptionTests
{
    private static IConfiguration BuildEmptyConfiguration()
        => new ConfigurationBuilder().Build();

    [Fact]
    public void MultipleDiAttributes_ThrowsMultipleDependencyInjectionAttributesPresentException()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            [Singleton<BadMultiAttr>]
            [Transient<BadMultiAttr>]
            public class BadMultiAttr { }
            """;

        var assembly = DynamicAssemblyHelper.CompileAndLoad(source, "MultiAttrTest");
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        Assert.Throws<MultipleDependencyInjectionAttributesPresentException>(
            () => services.AddAttributedServicesOf(config, assembly));
    }

    [Fact]
    public void ConditionMethodWrongReturnType_ThrowsConditionMethodHasInvalidSignatureException()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            [Singleton<BadReturnType>]
            public class BadReturnType
            {
                [DependencyInjectionCondition]
                public static int IsEnabled() => 1;
            }
            """;

        var assembly = DynamicAssemblyHelper.CompileAndLoad(source, "BadReturnTypeTest");
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        Assert.Throws<ConditionMethodHasInvalidSignatureException>(
            () => services.AddAttributedServicesOf(config, assembly));
    }

    [Fact]
    public void ConditionMethodWrongParamType_ThrowsConditionMethodHasInvalidSignatureException()
    {
        var source = """
            using X39.Util.DependencyInjection.Attributes;

            [Singleton<BadParamType>]
            public class BadParamType
            {
                [DependencyInjectionCondition]
                public static bool IsEnabled(string name) => true;
            }
            """;

        var assembly = DynamicAssemblyHelper.CompileAndLoad(source, "BadParamTypeTest");
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        Assert.Throws<ConditionMethodHasInvalidSignatureException>(
            () => services.AddAttributedServicesOf(config, assembly));
    }

    [Fact]
    public void ConditionMethodTooManyParams_ThrowsConditionMethodHasInvalidSignatureException()
    {
        var source = """
            using Microsoft.Extensions.Configuration;
            using X39.Util.DependencyInjection.Attributes;

            [Singleton<TooManyParams>]
            public class TooManyParams
            {
                [DependencyInjectionCondition]
                public static bool IsEnabled(IConfiguration config, string extra) => true;
            }
            """;

        var assembly = DynamicAssemblyHelper.CompileAndLoad(source, "TooManyParamsTest");
        var services = new ServiceCollection();
        var config = BuildEmptyConfiguration();

        Assert.Throws<ConditionMethodHasInvalidSignatureException>(
            () => services.AddAttributedServicesOf(config, assembly));
    }
}
