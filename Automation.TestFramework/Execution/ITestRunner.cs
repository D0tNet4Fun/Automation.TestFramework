using System.Threading.Tasks;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal interface ITestRunner
    {
        Task<RunSummary> RunAsync();
    }
}