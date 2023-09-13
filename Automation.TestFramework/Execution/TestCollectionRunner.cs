using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class TestCollectionRunner : XunitTestCollectionRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;

        private readonly IDictionary<Type, object> _assemblyFixtureMappings;
        private readonly Type _assemblyTestNotificationType;
        private readonly int _maxParallelThreads;

        public TestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> assemblyFixtureMappings, Type assemblyTestNotificationType, int maxParallelThreads)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            _assemblyFixtureMappings = assemblyFixtureMappings;
            _assemblyTestNotificationType = assemblyTestNotificationType;
            _maxParallelThreads = maxParallelThreads;
        }

        protected override Task<RunSummary> RunTestClassesAsync()
        {
            // run the test cases in parallel, unless parallelization is disabled by the user

            bool IsParallelizationDisabled()
            {
                // check if the collection definition class has [TestCaseCollectionOptions(ExecutionMode=...)]
                var testCollectionOptionsAttr = TestCollection.CollectionDefinition?.GetCustomAttributes(typeof(TestCaseCollectionOptionsAttribute)).SingleOrDefault();
                var testCaseExecutionMode = testCollectionOptionsAttr?.GetNamedArgument<TestCaseExecutionMode>(nameof(TestCaseCollectionOptionsAttribute.ExecutionMode));
                var isTestCaseExecutionModeSpecified = testCaseExecutionMode != null;

                // check if parallelism is disabled at the assembly level, meaning [assembly: CollectionBehavior(DisableTestParallelization=true)]
                var collectionBehaviorAttr = TestCollection.TestAssembly.Assembly.GetCustomAttributes(typeof(CollectionBehaviorAttribute)).SingleOrDefault();
                var disableParallelization = collectionBehaviorAttr?.GetNamedArgument<bool>(nameof(CollectionBehaviorAttribute.DisableTestParallelization));
                if (disableParallelization == true)
                {
                    if (isTestCaseExecutionModeSpecified)
                    {
                        // warn user that ExecutionMode will be ignored
                        _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case collection {TestCollection.DisplayName} defines execution mode {testCaseExecutionMode}, but this will be ignored because parallelization is disabled at the assembly level."));
                    }
                    return true;
                }

                // check if the collection is parallel-disabled, meaning [CollectionDefinition(DisableParallelization=true)]
                var collectionDefinitionAttr = TestCollection.CollectionDefinition?.GetCustomAttributes(typeof(CollectionDefinitionAttribute)).SingleOrDefault();
                disableParallelization = collectionDefinitionAttr?.GetNamedArgument<bool>(nameof(CollectionDefinitionAttribute.DisableParallelization));
                if (disableParallelization == true)
                {
                    if (isTestCaseExecutionModeSpecified)
                    {
                        // warn user that ExecutionMode will be ignored
                        _diagnosticMessageSink.OnMessage(new DiagnosticMessage($"Test case collection {TestCollection.DisplayName} defines execution mode {testCaseExecutionMode}, but this will be ignored because parallelization is disabled in the collection definition."));
                    }
                    return true;
                }

                return testCaseExecutionMode switch
                {
                    TestCaseExecutionMode.Sequential => true, // user wants to run the collection test cases sequentially
                    _ => false // no known restrictions
                };
            }

            return IsParallelizationDisabled() ? base.RunTestClassesAsync() : RunTestClassesInParallelAsync();
        }

        private async Task<RunSummary> RunTestClassesInParallelAsync()
        {
            var summary = new RunSummary();

            var parallelOptions = new ParallelOptions
            {
                CancellationToken = CancellationTokenSource.Token,
                MaxDegreeOfParallelism = _maxParallelThreads
            };

            var testCasesGroupedByClass = TestCases.GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance);
            var task = testCasesGroupedByClass.ForEachAsync(parallelOptions,
                async testCasesByClass =>
                {
                    var partialSummary = await RunTestClassAsync(testCasesByClass.Key, (IReflectionTypeInfo)testCasesByClass.Key.Class, testCasesByClass);
                    summary.Aggregate(partialSummary);
                });

            await task;
            return summary;
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