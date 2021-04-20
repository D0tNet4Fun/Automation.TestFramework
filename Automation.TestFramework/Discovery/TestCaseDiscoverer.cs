using Automation.TestFramework.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Discovery
{
    /// <summary>
    /// Extend xUnit's <see cref="FactDiscoverer"/> to find test methods in a class decorated with <see cref="TestCaseAttribute"/>.
    /// </summary>
    public class TestCaseDiscoverer : FactDiscoverer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDiscoverer" /> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        public TestCaseDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        /// <summary>
        /// Creates a single <see cref="IXunitTestCase" /> for the given test method.
        /// </summary>
        /// <param name="discoveryOptions">The discovery options to be used.</param>
        /// <param name="testMethod">The test method.</param>
        /// <param name="factAttribute">The attribute that decorates the test method.</param>
        protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
            => new TestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod);
    }
}