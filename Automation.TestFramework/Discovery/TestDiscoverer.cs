using System.Collections.Generic;
using System.Linq;
using Automation.TestFramework.Entities;
using Xunit.Abstractions;
using ITest = Automation.TestFramework.Entities.ITest;

namespace Automation.TestFramework.Discovery
{
    internal class TestDiscoverer
    {
        private readonly ITestCase _testCase;

        public TestDiscoverer(ITestCase testCase)
        {
            _testCase = testCase;
        }

        public IReadOnlyList<ITest> DiscoverTests()
        {
            var tests = new List<ITest>();

            // look for other methods in the test class
            var testClass = _testCase.TestMethod.TestClass;
            var methods = testClass.Class.GetMethods(includePrivateMethods: true);

            foreach (var methodInfo in methods)
            {
                var attributeInfo = methodInfo.GetCustomAttributes(typeof(TestCaseComponentAttribute)).SingleOrDefault();
                var isTestCaseComponent = attributeInfo != null;
                if (!isTestCaseComponent) continue;

                // create a test from this test method and link it to the test case
                var test = new Test(_testCase, methodInfo, methodInfo.Name); // todo name
                tests.Add(test);
            }

            return tests;
        }
    }
}