using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal interface IExpectedResultTest : ITest
    {
        bool ContinueOnError { get; }
    }

    internal class ExpectedResultTest : Test, IExpectedResultTest
    {
        public ExpectedResultTest(ITestCase testCase, object instance, IMethodInfo methodInfo, string displayName, bool continueOnError)
            : base(testCase, instance, methodInfo, displayName)
        {
            ContinueOnError = continueOnError;
        }

        public bool ContinueOnError { get; }
    }
}