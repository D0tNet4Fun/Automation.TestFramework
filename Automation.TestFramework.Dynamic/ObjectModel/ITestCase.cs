using System.Collections.Generic;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal interface ITestCase : IXunitTestCase
{
    public int StepCount { get; }

    public IReadOnlyCollection<Step> GetSteps();
    public string GetNextDynamicTestUniqueId();
}