using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic;

public interface ITestCase
{
    ITestCase AddStep(StepType stepType, string description, Action action);
    
    ITestCase AddAsyncStep(StepType stepType, string description, Func<Task> action);
    
    //void Run();
}