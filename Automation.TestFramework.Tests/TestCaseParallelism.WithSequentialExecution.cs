using Xunit;
using Xunit.Abstractions;

// define 2 test case collections with execution mode = Sequentially, define 2 tests in each collections and verify that they run in parallel 2 at a time

namespace Automation.TestFramework.Tests.Parallel.WithSequentialExecution
{
    [CollectionDefinition(Name)]
    [TestCaseCollectionOptions(ExecutionMode = TestCaseExecutionMode.Sequential)]
    public class TestCollection1
    {
        public const string Name = "Sequential1";
    }

    [CollectionDefinition(Name)]
    [TestCaseCollectionOptions(ExecutionMode = TestCaseExecutionMode.Sequential)]
    public class TestCollection2
    {
        public const string Name = "Sequential2";
    }

    [TestCase("TC1_1")]
    [Collection(TestCollection1.Name)]
    public class TC1_1 : TestCaseWorkerBase
    {
        public TC1_1(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC1_1_Summary() { }
        [Input] private void Input() => DoWork();
    }

    [TestCase("TC1_2")]
    [Collection(TestCollection1.Name)]
    public class TC1_2 : TestCaseWorkerBase
    {
        public TC1_2(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC1_2_Summary() { }
        [Input] private void Input() => DoWork();
    }

    [TestCase("TC2_1")]
    [Collection(TestCollection2.Name)]
    public class TC2_1 : TestCaseWorkerBase
    {
        public TC2_1(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC2_1_Summary() { }
        [Input] private void Input() => DoWork();
    }

    [TestCase("TC2_2")]
    [Collection(TestCollection2.Name)]
    public class TC2_2 : TestCaseWorkerBase
    {
        public TC2_2(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC2_2_Summary() { }
        [Input] private void Input() => DoWork();
    }
}
