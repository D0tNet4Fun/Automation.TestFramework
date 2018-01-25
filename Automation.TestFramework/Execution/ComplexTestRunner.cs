using System;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class ComplexTestRunner : ITestRunner
    {
        private readonly ITest _complexTest;
        private readonly Func<ITest, TestRunner> _testRunnerFactory;

        public ComplexTestRunner(ITest complexTest, Func<ITest, TestRunner> testRunnerFactory)
        {
            _complexTest = complexTest;
            _testRunnerFactory = testRunnerFactory;
        }

        public async Task<RunSummary> RunAsync()
        {
            var sumamry = new RunSummary();
            foreach (var action in _complexTest.Actions)
            {
                // actions which are not to be included in the test report need to be invoked directly
                // the others are invoked using test runners
                if (!action.ShowInTestReport)
                {
                    InvokeAction(action);
                }
                else
                {
                    var testRunner = _testRunnerFactory(action);
                    sumamry.Aggregate(await testRunner.RunAsync());
                }
            }
            return sumamry;
        }

        private void InvokeAction(ITest action)
        {
            var method = action.MethodInfo.ToRuntimeMethod();
            method.Invoke(action.TestClassInstance, null);
        }
    }
}