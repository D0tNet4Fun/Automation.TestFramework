using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Discovery
{
    public class TestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public TestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            IXunitTestCase testCase;

            if (testMethod.Method.IsStatic)
                testCase = new ExecutionErrorTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod,
                    "[Summary] methods are not allowed to be static.");
            else if (testMethod.Method.GetParameters().Any())
                testCase = new ExecutionErrorTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod,
                    "[Summary] methods are not allowed to have parameters.");
            else if (testMethod.Method.IsGenericMethodDefinition)
                testCase = new ExecutionErrorTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod,
                    "[Summary] methods are not allowed to be generic.");
            else
                testCase = CreateTestCase(discoveryOptions, testMethod, factAttribute);

            return new[] { testCase };
        }

        private IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
            => new TestCase(discoveryOptions.MethodDisplayOrDefault(), testMethod);
    }
}