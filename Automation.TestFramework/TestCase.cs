using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using Automation.TestFramework.Execution;

namespace Automation.TestFramework
{
    internal class TestCase : TestMethodTestCase, IXunitTestCase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes", error: true)]
        public TestCase()
        {
        }

        public TestCase(TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null)
            : base(defaultMethodDisplay, testMethod, testMethodArguments)
        {
        }

        public Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            => new TestCaseRunner(this, DisplayName, SkipReason, constructorArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
    }
}