using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Discovery
{
    public class TestCaseDiscoverer : FactDiscoverer
    {
        public TestCaseDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
            => new TestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
    }
}