using System.Collections.Generic;
using System.Linq;
using Automation.TestFramework.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Discovery
{
    internal class TestCaseComponentDiscoverer
    {
        public ITestClass TestClass { get; }
        public IMessageSink DiagnosticMessageSink { get; }
        public TestMethodDisplay MethodDisplay { get; }
        public ISourceInformationProvider SourceInformationProvider { get; }

        public TestCaseComponentDiscoverer(ITestClass testClass, IMessageSink diagnosticMessageSink, TestMethodDisplay methodDisplay, ISourceInformationProvider sourceInformationProvider)
        {
            TestClass = testClass;
            DiagnosticMessageSink = diagnosticMessageSink;
            MethodDisplay = methodDisplay;
            SourceInformationProvider = sourceInformationProvider;
        }

        public IEnumerable<IXunitTestCase> Discover()
        {
            foreach (var methodInfo in TestClass.Class.GetMethods(includePrivateMethods: true))
            {
                var testCase = Discover(methodInfo);
                if (testCase != null) yield return testCase;
            }
        }

        private IXunitTestCase Discover(IMethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttributes(typeof(TestCaseComponentAttribute)).SingleOrDefault();
            if (attribute == null) return null;

            var testCase = new TestCase(DiagnosticMessageSink, MethodDisplay, new TestMethod(TestClass, methodInfo));
            if (SourceInformationProvider != null)
                testCase.SourceInformation = SourceInformationProvider.GetSourceInformation(testCase);
            return testCase;
        }
    }
}