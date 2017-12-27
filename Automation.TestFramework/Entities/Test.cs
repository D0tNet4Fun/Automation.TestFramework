using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal interface ITest : Xunit.Abstractions.ITest
    {
        IMethodInfo MethodInfo { get; }

        new string DisplayName { get; set; }
    }

    internal class Test : ITest
    {
        public Test(ITestCase testCase, IMethodInfo methodInfo, string displayName)
        {
            TestCase = testCase;
            MethodInfo = methodInfo;
            DisplayName = displayName;
        }

        public ITestCase TestCase { get; }
        public IMethodInfo MethodInfo { get; }

        public string DisplayName { get; set; }
    }
}