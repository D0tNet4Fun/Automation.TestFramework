using System;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class ExpectedResultTestRunner : ITestRunner
    {
        private readonly ITestWithExpectedResult _test;
        private readonly Func<ITest, ITestRunner> _testRunnerFactory;
        private readonly Func<ITest, Exception, RunSummary> _failTestOnExceptionCallback;

        public ExpectedResultTestRunner(ITestWithExpectedResult test, Func<ITest, ITestRunner> testRunnerFactory, Func<ITest, Exception, RunSummary> failTestOnExceptionCallback)
        {
            _test = test;
            _testRunnerFactory = testRunnerFactory;
            _failTestOnExceptionCallback = failTestOnExceptionCallback;
        }

        public async Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();

            // run the test first to collect all the expected result actions; if it fails then exit.
            var runner = _testRunnerFactory(_test);
            summary.Aggregate(await runner.RunAsync());
            if (summary.Failed > 0) return summary;

            var actions = ExpectedResult.Current.Actions;
            if (actions.Length == 0)
            {
                var exception = new Exception($"Expected result should have at least one action: ({nameof(IExpectedResult.Assert)}) or ({nameof(IExpectedResult.Verify)})");
                summary.Aggregate(_failTestOnExceptionCallback(_test, exception));
                return summary;
            }

            var index = 1;
            foreach (var action in actions)
            {
                var displayName = $"{_test.DisplayNamePrefix}{index++}. {action.Description}";
                var test = new ExpectedResultTest(_test.TestCase, action.Action.Target, new ReflectionMethodInfo(action.Action.Method), displayName, action.ContinueOnError);
                runner = _testRunnerFactory(test);
                summary.Aggregate(await runner.RunAsync());
                if (summary.Failed > 0 && !action.ContinueOnError)
                {
                    break;
                }
            }
            return summary;
        }
    }
}