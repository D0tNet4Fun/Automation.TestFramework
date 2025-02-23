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

        var runSummary = await Run(context);

        if (context.SubStepsRunSummary is not null)
        {
            runSummary.Aggregate(context.SubStepsRunSummary.Value);
        }

        return runSummary;
    }

    protected override void PreInvoke(StepRunnerContext ctxt)
    {
        ctxt.Step.RunnerContext = ctxt;
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
            ctxt.Step.RunnerContext = null;
        }
    }
}