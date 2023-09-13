using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework.Tests.Parallel.Mixed
{
    [CollectionDefinition(Name)]
    [TestCaseCollectionOptions(ExecutionMode = TestCaseExecutionMode.InParallelIfAllowed)]
    public class TestCollection1
    {
        public const string Name = "ParallelEnabled_ParallelExecution";
    }

    [CollectionDefinition(Name)]
    [TestCaseCollectionOptions(ExecutionMode = TestCaseExecutionMode.Sequential)]
    public class TestCollection2
    {
        public const string Name = "ParallelEnabled_SequentialExecution";
    }

    [CollectionDefinition(Name, DisableParallelization = true)]
    //[TestCaseCollectionOptions(ExecutionMode = TestCaseExecutionMode.InParallelIfAllowed)] // this should issue a warning at run time because parallelization is disabled
    public class TestCollection3
    {
        public const string Name = "ParallelDisabled";
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

    [TestCase("TC3_1")]
    [Collection(TestCollection3.Name)]
    public class TC3_1 : TestCaseWorkerBase
    {
        public TC3_1(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC3_1_Summary() { }
        [Input] private void Input() => DoWork();
    }

    [TestCase("TC3_2")]
    [Collection(TestCollection3.Name)]
    public class TC3_2 : TestCaseWorkerBase
    {
        public TC3_2(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Summary] public void TC3_2_Summary() { }
        [Input] private void Input() => DoWork();
    }
}
