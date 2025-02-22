using System;

namespace Automation.TestFramework.Dynamic.ObjectModel;

public interface IExpectedResult : IStep
{
    public IExpectedResult Assert(string description, Action action);
}