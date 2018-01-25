using System;
using System.Collections.Generic;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
{
    public interface ITestStep
    {
        ITestStep Initialize(Action action);

        ITestStep Do(string actionDescription, Action action);
    }

    public class TestStep : ITestStep
    {
        private readonly List<TestStepAction> _testStepActions = new List<TestStepAction>();

        public ITestStep Initialize(Action action)
        {
            _testStepActions.Add(new TestStepAction(string.Empty, action, showInTestReport: false));
            return this;
        }

        public ITestStep Do(string actionDescription, Action action)
        {
            _testStepActions.Add(new TestStepAction(actionDescription, action, showInTestReport: true));
            return this;
        }

        internal IEnumerable<TestStepAction> Actions => _testStepActions;
    }
}