using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class StepRunner : DynamicTestRunnerBase<StepRunnerContext>
{
    public static StepRunner Instance { get; } = new();

    public async Task<RunSummary> Run(
        ObjectModel.Step step,
        IDynamicTest test, 
        IMessageBus messageBus, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource)
    {
        await using var context = new StepRunnerContext(
            step,
            test,
            messageBus,
            aggregator,
            cancellationTokenSource);

        await context.InitializeAsync();

        return await Run(context);
    }

    protected override void PreInvoke(StepRunnerContext ctxt)
    {
        ctxt.Step.PreInvoke();
        base.PreInvoke(ctxt);
    }

    protected override void PostInvoke(StepRunnerContext ctxt)
    {
        try
        {
            base.PostInvoke(ctxt);
        }
        finally
        {
            ctxt.Step.PostInvoke();
        }
    }
}