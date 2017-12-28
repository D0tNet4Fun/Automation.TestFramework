using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class TestClassRunner : XunitTestClassRunner
    {
        private Type _testNotificationType;

        public TestClassRunner(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, IDictionary<Type, object> collectionFixtureMappings, Type assemblyTestNotificationType)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource, collectionFixtureMappings)
        {
            _testNotificationType = assemblyTestNotificationType;
        }

        protected override async Task AfterTestClassStartingAsync()
        {
            await base.AfterTestClassStartingAsync();

            GetClassTestNotificationType();
        }

        private void GetClassTestNotificationType()
        {
            var testNotificationAttribute = ((IReflectionTypeInfo)TestClass.Class).Type
                .GetCustomAttributes(typeof(TestNotificationAttribute), false)
                .Cast<TestNotificationAttribute>()
                .SingleOrDefault();

            // override the assembly test notification type
            if (testNotificationAttribute != null)
                _testNotificationType = testNotificationAttribute.Type;
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, object[] constructorArguments)
            => new TestMethodRunner(testMethod, Class, method, testCases, DiagnosticMessageSink, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments, ClassFixtureMappings, _testNotificationType).RunAsync();
    }
}