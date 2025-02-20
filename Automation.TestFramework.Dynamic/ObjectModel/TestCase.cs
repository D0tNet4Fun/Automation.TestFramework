using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class TestCase(XunitTestCase xtc, string displayName)
    : XunitTestCase(xtc.TestMethod, displayName, xtc.UniqueID, xtc.Explicit, xtc.SkipReason, xtc.SkipType, xtc.SkipUnless, xtc.SkipWhen, xtc.Traits, xtc.TestMethodArguments, xtc.SourceFilePath, xtc.SourceLineNumber, xtc.Timeout),
    ISelfExecutingXunitTestCase
{
    public override void PreInvoke()
    {
        Automation.TestFramework.TestCase.SetCurrent();
        base.PreInvoke();
    }

    public async ValueTask<RunSummary> Run(ExplicitOption explicitOption, IMessageBus messageBus, object?[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        // messageBus = new MessageBusWrapper(messageBus); // for debugging
        
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
        Automation.TestFramework.TestCase.ResetCurrent();
        base.PostInvoke();
    }
}