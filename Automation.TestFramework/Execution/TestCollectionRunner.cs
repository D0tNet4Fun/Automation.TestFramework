using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class TestCollectionRunner : XunitTestCollectionRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;

        private readonly IDictionary<Type, object> _assemblyFixtureMappings;
        private readonly Type _assemblyTestNotificationType;

        public TestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> assemblyFixtureMappings, Type assemblyTestNotificationType)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            _assemblyFixtureMappings = assemblyFixtureMappings;
            _assemblyTestNotificationType = assemblyTestNotificationType;
        }

        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
        {
            // Don't want to use .Concat + .ToDictionary because of the possibility of overriding types,
            // so instead we'll just let collection fixtures override assembly fixtures.
            var combinedFixtures = new Dictionary<Type, object>(_assemblyFixtureMappings);
            foreach (var kvp in CollectionFixtureMappings)
                combinedFixtures[kvp.Key] = kvp.Value;

            return new TestClassRunner(testClass, @class, testCases, _diagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, combinedFixtures, _assemblyTestNotificationType).RunAsync();
        }
    }
}