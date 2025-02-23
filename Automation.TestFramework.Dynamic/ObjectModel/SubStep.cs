using System;
using Xunit.Internal;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class SubStep(SubStepType type, int order, string description, Delegate code)
{
    public SubStepType Type { get; } = type;
    public string Description { get; } = Guard.ArgumentNotNull(description, nameof(description));
    public Delegate Code { get; } = Guard.ArgumentNotNull(code, nameof(code));
    
    public IDynamicTest ToXunitTest()
    {
        var testCase = TestCase.Current;
        var step = Step.Current;

        return new DynamicTest(
            testCase,
            GetTestDisplayName(),
            uniqueId: testCase.GetNextDynamicTestUniqueId(),
            timeout: null,
            Code);
        
        string GetTestDisplayName()
        {
            // sample: [3/10] [Expected result] 1.1. This should work
            // sample: [3/10] [Expected result] 1.2. This should work too
            var typeDescription = $"{step.Type.GetDisplayName()}";
            var testDisplayName = $"[{step.Index}/{testCase.StepCount}] [{typeDescription}] {step.Order}.{order}. {Description}";

            return testDisplayName;
        }
    }
}