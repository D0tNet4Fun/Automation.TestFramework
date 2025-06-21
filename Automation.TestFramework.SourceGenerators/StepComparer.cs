using System.Collections.Generic;
using Automation.TestFramework.SourceGenerators.ObjectModel;

namespace Automation.TestFramework.SourceGenerators;

internal class StepComparer : IComparer<Step>
{
    public int Compare(Step x, Step y)
    {
        if (ReferenceEquals(x, y)) return 0;

        // if same type, then order matters
        if (x.Type == y.Type)
        {
            return x.Order.CompareTo(y.Order);
        }

        // else they are different types
        // special cases when comparing input and expected result
        if (x.Type == StepType.Input && y.Type == StepType.ExpectedResult)
        {
            // input goes first, unless expected result has a lower order
            return x.Order <= y.Order ? -1 : 1;
        }

        if (x.Type == StepType.ExpectedResult && y.Type == StepType.Input)
        {
            // expected results goes first, unless the input has a higher order
            return x.Order < y.Order ? -1 : 1; // same as Compare(y, x)
        }

        // else compare by type
        return x.Type.CompareTo(y.Type);
    }
}