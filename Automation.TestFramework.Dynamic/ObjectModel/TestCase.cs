using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class TestCase(XunitTestCase xtc, string displayName)
    : XunitTestCase(xtc.TestMethod, displayName, xtc.UniqueID, xtc.Explicit, xtc.SkipReason, xtc.SkipType, xtc.SkipUnless, xtc.SkipWhen, xtc.Traits, xtc.TestMethodArguments, xtc.SourceFilePath, xtc.SourceLineNumber, xtc.Timeout),
    IDynamicTestCase, ITestCase, ISelfExecutingXunitTestCase
{
    /// <summary>
    /// Value used only when computing a unique ID for a Xunit test
    /// </summary>
    private int _nextTestIndex = 1;
    private readonly TestCaseDescriptor _descriptor = new();

    public static ITestCase Current => (ITestCase)TestFramework.TestCase.Current;

    public ITestCaseDescriptor Descriptor => _descriptor;
    public int StepCount => _descriptor.StepCount;
    public IReadOnlyCollection<Step> GetSteps() => _descriptor.GetSteps();

    public string GetNextDynamicTestUniqueId() => UniqueIDGenerator.ForTest(xtc.UniqueID, _nextTestIndex++);

    public override void PreInvoke()
    {
        TestFramework.TestCase.SetCurrent(this);
        base.PreInvoke();
    }

    public async ValueTask<RunSummary> Run(ExplicitOption explicitOption, IMessageBus messageBus, object?[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        messageBus = new MessageBusWrapper(messageBus); // for debugging

        var tests = await aggregator.RunAsync(CreateTests, []);

        return await TestCaseRunner.Instance.Run(
            this,
            tests,
            explicitOption,
            messageBus,
            constructorArguments,
            aggregator,
            cancellationTokenSource);
    }

    public override void PostInvoke()
    {
        try
        {
            base.PostInvoke();
        }
        finally
        {
            TestFramework.TestCase.ResetCurrent();
        }
    }
}