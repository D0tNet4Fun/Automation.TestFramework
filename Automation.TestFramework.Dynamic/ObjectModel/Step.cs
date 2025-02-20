using System;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class Step
{
    public Step(StepType type, int order, string description, Delegate code)
    {
        if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));

        Type = type;
        Order = order;
        Description = description;
        Code = code;
    }

    public StepType Type { get; }

    public int Order { get; }

    public string Description { get; }

    public Delegate Code { get; }

    public int? Timeout { get; set; }

    public IDynamicTest ToXunitTest(IXunitTestCase testCase, int index, int count)
    {
        var displayName = GetTestDisplayName();

        return new DynamicTest(
            testCase,
            displayName,
            uniqueID: UniqueIDGenerator.ForTest(testCase.UniqueID, index),
            Timeout,
            Code);

        string GetTestDisplayName()
        {
            // sample: [1/10] [Input] 1. Do something
            var typeDescription = Type.GetDisplayName();
            var testDisplayName = $"[{index}/{count}] {typeDescription}: {Order}. {Description}";

            return testDisplayName;
        }
    }
}