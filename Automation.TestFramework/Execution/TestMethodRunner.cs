using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class TestMethodRunner : XunitTestMethodRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;
        private readonly object[] _constructorArguments;
        private readonly Dictionary<Type, object> _classFixtureMappings;

        public TestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments, Dictionary<Type, object> classFixtureMappings)
            : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            _constructorArguments = constructorArguments;
            _classFixtureMappings = classFixtureMappings;
        }

        protected override Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
        {
            if (testCase is TestCase customTestCase)
                customTestCase.SetClassFixtureMappings(_classFixtureMappings);
            return testCase.RunAsync(_diagnosticMessageSink, MessageBus, _constructorArguments, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }
    }
}