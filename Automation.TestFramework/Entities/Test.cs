using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal interface ITest : Xunit.Abstractions.ITest
    {
        object TestClassInstance { get; set; }

        IMethodInfo MethodInfo { get; }

        new string DisplayName { get; set; }

        List<ITest> Actions { get; set; }

        bool ShowInTestReport { get; }
    }

    internal class Test : LongLivedMarshalByRefObject, ITest
    {
        public Test(ITestCase testCase, object testClassInstance, IMethodInfo methodInfo, string displayName, bool showInTestReport = true)
        {
            TestCase = testCase;
            TestClassInstance = testClassInstance;
            MethodInfo = methodInfo;
            DisplayName = displayName;
            ShowInTestReport = showInTestReport;
        }

        public ITestCase TestCase { get; }

        public object TestClassInstance { get; set; }

        public IMethodInfo MethodInfo { get; }

        public string DisplayName { get; set; }

        public bool ShowInTestReport { get; }

        public List<ITest> Actions { get; set; } = new List<ITest>();
    }
}