using System.Reflection;
using Automation.TestFramework.Discovery;
using Automation.TestFramework.Execution;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework
{
    public class Framework : XunitTestFramework
    {
        public Framework(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
            => new FrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new FrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
