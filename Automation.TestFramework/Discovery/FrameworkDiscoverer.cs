using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Discovery
{
    internal class FrameworkDiscoverer : XunitTestFrameworkDiscoverer
    {
        public FrameworkDiscoverer(IAssemblyInfo assemblyInfo, ISourceInformationProvider sourceProvider, IMessageSink diagnosticMessageSink, IXunitTestCollectionFactory collectionFactory = null)
            : base(assemblyInfo, sourceProvider, diagnosticMessageSink, collectionFactory)
        {
        }

        protected override bool IsValidTestClass(ITypeInfo type)
        {
            // only classes marked as test cases are valid
            var isTestCase = type.GetCustomAttributes(typeof(TestCaseAttribute)).Any();
            if (!isTestCase)
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Class {type.Name} is not a test case."));
            return isTestCase;
        }

        protected override bool FindTestsForMethod(ITestMethod testMethod, bool includeSourceInformation, IMessageBus messageBus, ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"finding for method {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));

            // only look for methods marked as Summary
            var summaryAttribute = testMethod.Method.GetCustomAttributes(typeof(SummaryAttribute)).SingleOrDefault();
            if (summaryAttribute == null)
            {
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage("skip test case:" + testMethod.Method.Name));
                return true;
            }

            var discoverer = new TestCaseDiscoverer(DiagnosticMessageSink);

            foreach (var testCase in discoverer.Discover(discoveryOptions, testMethod, summaryAttribute))
            {
                DiagnosticMessageSink.OnMessage(new DiagnosticMessage("Found test case:" + testCase.Method.Name));
                if (!ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus))
                    return false;
            }

            return true;
        }
    }
}