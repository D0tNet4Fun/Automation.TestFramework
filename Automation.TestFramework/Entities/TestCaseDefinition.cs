using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal class TestCaseDefinition
    {
        private readonly ITestCase _testCase;
        private readonly ITestClass _testClass;
        private readonly List<ITest> _preconditions = new List<ITest>();
        private readonly List<TestStep> _testSteps = new List<TestStep>();

        public TestCaseDefinition(ITestCase testCase)
        {
            _testCase = testCase;
            _testClass = testCase.TestMethod.TestClass;
        }

        public IList<ITest> Preconditions => _preconditions;

        public IList<TestStep> Steps => _testSteps;

        public void DiscoverTestCaseComponents()
        {
            var testMethods = GetTestMethods();

            // get the test methods which are test case components, and their attribute
            var testCaseComponents =
                (from testMethod in testMethods
                 let attributeInfo = testMethod.GetCustomAttributes(typeof(TestCaseComponentAttribute)).SingleOrDefault()
                 where attributeInfo != null
                 let attribute = (TestCaseComponentAttribute)(attributeInfo as IReflectionAttributeInfo)?.Attribute
                 where attribute != null
                 select new { testMethod, attribute })
                .ToList();

            // initialize the lists
            var count = testCaseComponents.Count(x => x.attribute is PreconditionAttribute);
            _preconditions.AddRange(Enumerable.Range(0, count).Select(_ => (ITest)null));
            count = testCaseComponents.Count(x => x.attribute is InputAttribute);
            _testSteps.AddRange(Enumerable.Range(0, count).Select(_ => new TestStep()));

            // distribute the test case components into the lists
            foreach (var pair in testCaseComponents)
            {
                var attribute = pair.attribute;
                var index = pair.attribute.Order - 1;
                var test = CreateTest(pair.testMethod, attribute);

                if (attribute is PreconditionAttribute)
                {
                    _preconditions[index] = test;
                }
                else if (attribute is InputAttribute)
                {
                    _testSteps[index].Input = test;
                }
                else if (attribute is ExpectedResultAttribute)
                {
                    _testSteps[index].ExpectedResult = test;
                }
            }
        }

        private IEnumerable<IMethodInfo> GetTestMethods()
        {
            return _testClass.Class.GetMethods(includePrivateMethods: true);
        }

        private ITest CreateTest(IMethodInfo testMethod, TestCaseComponentAttribute attribute)
        {
            // create a test from this test method and assign it to the test case
            var displayName = attribute.DisplayName;
            return new Test(_testCase, testMethod, displayName);
        }
    }
}