using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

/// <summary>
/// Allows adding steps from code, using a fluent API.
/// </summary>
public interface ITestCaseDescriptor
{
    ITestCaseDescriptor AddStep(StepType stepType, string description, Action code);
    
    ITestCaseDescriptor AddAsyncStep(StepType stepType, string description, Func<Task> code);
    
    ITestCaseDescriptor AddAsyncStep(StepType stepType, string description, Func<ValueTask> code);
    
    //void Run();
}