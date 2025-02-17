using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Execution;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Entities;

public class TestCase(XunitTestCase xtc, string displayName)
    : XunitTestCase(xtc.TestMethod, displayName, xtc.UniqueID, xtc.Explicit, xtc.SkipReason, xtc.SkipType, xtc.SkipUnless, xtc.SkipWhen, xtc.Traits, xtc.TestMethodArguments, xtc.SourceFilePath, xtc.SourceLineNumber, xtc.Timeout)
    , ITestCase, ISelfExecutingXunitTestCase
{
    private static readonly AsyncLocal<ITestCase?> Local = new();
    
    private readonly List<Step> _steps = [];

    public static ITestCase Current => Local.Value ?? throw new InvalidOperationException("Current test case is null");

    public ITestCase AddStep(StepType stepType, string description, Action code) =>
        AddStepFromDelegate(stepType, description, code);

    public ITestCase AddAsyncStep(StepType stepType, string description, Func<Task> code) =>
        AddStepFromDelegate(stepType, description, code);

    private ITestCase AddStepFromDelegate(StepType stepType, string description, Delegate code)
    {
        var count = stepType switch
        {
            StepType.ExpectedResult => _steps.Count(s => s.Type == StepType.Input),
            _ => _steps.Count(s => s.Type == stepType) + 1,
        };
        var step = new Step(stepType, count, description, code);
        _steps.Add(step);
        
        return this;
    }

    internal IReadOnlyCollection<IDynamicTest> CreateTestsFromSteps()
    {
        var tests = _steps
            .Select((step, index) => step.ToXunitTest(this, index + 1, _steps.Count))
            .ToArray();

        return tests;
    }

    public override void PreInvoke()
    {
        Local.Value = this;
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
        Local.Value = null;
        base.PostInvoke();
    }
}