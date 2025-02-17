using System.Reflection;
using Automation.TestFramework.Dynamic.Entities;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Discovery;

internal class TestCaseDiscoverer : FactDiscoverer
{
    /// <inheritdoc />
    protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, IFactAttribute factAttribute)
    {
        var xunitTestCase = (XunitTestCase)base.CreateTestCase(discoveryOptions, testMethod, factAttribute);
        
        var summaryAttribute = testMethod.Method.GetCustomAttribute<SummaryAttribute>();
        var displayName = GetTestCaseDisplayName(summaryAttribute, testMethod.Method);

        return new TestCase(xunitTestCase, displayName);
    }

    private static string GetTestCaseDisplayName(SummaryAttribute summaryAttribute, MethodInfo method)
    {
        var displayName = summaryAttribute.DisplayName;
        if (string.IsNullOrEmpty(displayName)) displayName = method.GetHumanizedName();

        return displayName!;
    }
}