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
    }
}