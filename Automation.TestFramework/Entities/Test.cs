using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal interface ITest : Xunit.Abstractions.ITest
    {
        object Instance { get; set; }

        IMethodInfo MethodInfo { get; }

        new string DisplayName { get; set; }
    }

    internal class Test : LongLivedMarshalByRefObject, ITest
    {
        public Test(ITestCase testCase, object instance, IMethodInfo methodInfo, string displayName)
        {
            TestCase = testCase;
            Instance = instance;
            MethodInfo = methodInfo;
            DisplayName = displayName;
        }

        public ITestCase TestCase { get; }

        public object Instance { get; set; }

        public IMethodInfo MethodInfo { get; }

        public string DisplayName { get; set; }
    }
}