using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class TestCase(XunitTestCase xtc, string displayName)
    : XunitTestCase(xtc.TestMethod, displayName, xtc.UniqueID, xtc.Explicit, xtc.SkipReason, xtc.SkipType, xtc.SkipUnless, xtc.SkipWhen, xtc.Traits, xtc.TestMethodArguments, xtc.SourceFilePath, xtc.SourceLineNumber, xtc.Timeout),
        ITestCase, ISelfExecutingXunitTestCase
{
    private readonly List<Step> _steps = [];

    /// <summary>
    /// Value used only when computing a unique ID for a Xunit test
    /// </summary>
    private int _nextTestIndex = 1;

    public static TestCase Current => (TestCase)Automation.TestFramework.TestCase.Current;
    
    public int StepCount => _steps.Count;

    public RuntimeDependencies? RuntimeDependencies { get; private set; }

    public ITestCase AddStep(StepType stepType, string description, Action code) =>
        AddStepFromDelegate(stepType, description, code);

    public ITestCase AddAsyncStep(StepType stepType, string description, Func<Task> code) =>
        AddStepFromDelegate(stepType, description, code);

    private ITestCase AddStepFromDelegate(StepType stepType, string description, Delegate code)
    {
        var index = _steps.Count + 1;
        
        var order = stepType switch
        {
            StepType.ExpectedResult => _steps.Count(s => s.Type == StepType.Input),
            _ => _steps.Count(s => s.Type == stepType) + 1,
        };

        var step = stepType switch
        {
            StepType.ExpectedResult => new ExpectedResult(index, order, description, code),
            _ => new Step(stepType, index, order, description, code)
        };
        _steps.Add(step);

        return this;
    }

    public IReadOnlyCollection<Step> GetSteps() => _steps.AsReadOnly();

    public int GetNextTestIndex() => _nextTestIndex++;

    public override void PreInvoke()
    {
        Automation.TestFramework.TestCase.SetCurrent(this);
        base.PreInvoke();
    }

    public async ValueTask<RunSummary> Run(ExplicitOption explicitOption, IMessageBus messageBus, object?[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        // messageBus = new MessageBusWrapper(messageBus); // for debugging
        
        // capture the provided runtime dependencies
        RuntimeDependencies = new RuntimeDependencies(messageBus, aggregator, cancellationTokenSource);

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
            Automation.TestFramework.TestCase.ResetCurrent();
        }
    }
}