using Automation.TestFramework.Discovery;
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
    }
}
