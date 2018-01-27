using System.Collections.Generic;
using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal interface ITestWithExpectedResult : ITest
    {
        List<IExpectedResultTest> Actions { get; set; }

        string DisplayNamePrefix { get; set; }
    }

    internal class TestWithExpectedResult : Test, ITestWithExpectedResult
    {
        public TestWithExpectedResult(ITestCase testCase, object instance, IMethodInfo methodInfo, string displayName)
            : base(testCase, instance, methodInfo, displayName)
        {
        }

        public List<IExpectedResultTest> Actions { get; set; } = new List<IExpectedResultTest>();

        public string DisplayNamePrefix { get; set; }
    }
}