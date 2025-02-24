using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class SubStepRunner
{
    public static SubStepRunner Instance { get; } = new();

    public async ValueTask Run(
        StepRunnerContext? stepRunnerContext,
        IReadOnlyCollection<SubStep> subSteps)
    {
        if (stepRunnerContext is null) throw new ArgumentNullException(nameof(stepRunnerContext));

        var subStepsRunSummary = stepRunnerContext.SubStepsRunSummary ?? new RunSummary();

        foreach (var subStep in subSteps)
        {
            var dynamicTest = subStep.ToXunitTest();
            // skip this test if there are critical errors from running previous sub-steps
            if (stepRunnerContext.HasCriticalErrors)
            {
                dynamicTest.SkipReason = "Skipped because errors occurred in previous sub-steps.";
            }

            var subStepRunSummary = await DynamicTestRunner.Instance.Run(dynamicTest, stepRunnerContext.MessageBus, stepRunnerContext.Aggregator, stepRunnerContext.CancellationTokenSource);
            subStepsRunSummary.Aggregate(subStepRunSummary);

            // acknowledge there are critical errors while running the current step, if this sub-step failed, and it's not a verification
            // this will cause all future sub-steps to be skipped
            stepRunnerContext.HasCriticalErrors = subStepsRunSummary.Failed > 0 && subStep.Type != SubStepType.Verification;
        }

        stepRunnerContext.SubStepsRunSummary = subStepsRunSummary;
    }
}