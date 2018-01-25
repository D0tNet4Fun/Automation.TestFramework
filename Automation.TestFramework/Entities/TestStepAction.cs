using System;

namespace Automation.TestFramework.Entities
{
    internal class TestStepAction
    {
        public string Description { get; }
        public Action Action { get; }
        public bool ShowInTestReport { get; }

        public TestStepAction(string description, Action action, bool showInTestReport)
        {
            Description = description;
            Action = action;
            ShowInTestReport = showInTestReport;
        }
    }
}