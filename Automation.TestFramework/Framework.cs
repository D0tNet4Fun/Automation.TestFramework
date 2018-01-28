using System.Reflection;
using Automation.TestFramework.Discovery;
using Automation.TestFramework.Execution;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework
{
    /// <summary>
    /// The extension of <see cref="XunitTestFramework" /> that supports discovery and execution of tests in classes decorated with <see cref="TestCaseAttribute"/>.
    /// </summary>
    public class Framework : XunitTestFramework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Framework" /> class.
        /// </summary>
        /// <param name="messageSink">The message sink used to send diagnostic messages</param>
        public Framework(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        /// <inheritdoc />
        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
            => new FrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

        /// <inheritdoc />
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new FrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
