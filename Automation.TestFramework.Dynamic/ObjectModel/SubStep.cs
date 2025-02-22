using System;
using Xunit.Sdk;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class SubStep
{
    public SubStepType Type { get; }
    public int Index { get; }
    public string Description { get; }
    public Delegate Code { get; }

    public SubStep(SubStepType type, int index, string description, Delegate code)
    {
        Type = type;
        Index = index;
        Description = description;
        Code = code;
    }

    public IDynamicTest ToXunitTest()
    {
        var testCase = TestCase.Current;
        var step = Step.Current;

        return new DynamicTest(
            testCase,
            GetTestDisplayName(),
            uniqueID: UniqueIDGenerator.ForTest(testCase.UniqueID, testCase.GetNextTestIndex()),
            timeout: null,
            Code);
        
        string GetTestDisplayName()
        {
            // sample: [3/10] [Expected result] 1.1. This should work
            // sample: [3/10] [Expected result] 1.2. This should work too
            var typeDescription = $"{step.Type.GetDisplayName()}";
            var testDisplayName = $"[{step.Index}/{testCase.StepCount}] [{typeDescription}] {step.Order}.{Index}. {Description}";

            return testDisplayName;
        }
    }
}