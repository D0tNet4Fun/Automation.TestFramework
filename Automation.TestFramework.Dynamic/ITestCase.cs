using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

public interface ITestCase
{
    ITestCase AddStep(StepType stepType, string description, Action action);
    
    ITestCase AddAsyncStep(StepType stepType, string description, Func<Task> action);
    
    //void Run();
}