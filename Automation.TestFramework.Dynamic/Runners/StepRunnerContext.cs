using System.Threading;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class StepRunnerContext(
    ObjectModel.Step step,
    IDynamicTest test,
    object? testClassInstance,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource)
    : XunitTestRunnerBaseContext<IDynamicTest>(
        test, messageBus, ExplicitOption.Off, aggregator, cancellationTokenSource,
        beforeAfterTestAttributes: [], // not needed 
        constructorArguments: [] // not needed
    )
{
    public ObjectModel.Step Step { get; } = step;

    public object? TestClassInstance { get; } = testClassInstance;

    public RunSummary? SubStepsRunSummary { get; set; }

    public bool HasCriticalErrors { get; set; }
}