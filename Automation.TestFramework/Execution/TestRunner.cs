using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using ITest = Automation.TestFramework.Entities.ITest;

namespace Automation.TestFramework.Execution
{
    internal class TestRunner : TestRunner<ITestCase>
    {
        private readonly object _testClassInstance;

        public TestRunner(object testClassInstance, ITest test, IMessageBus messageBus, Type testClass, MethodInfo testMethod, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, new object[0], testMethod, new object[0], string.Empty, aggregator, cancellationTokenSource)
        {
            _testClassInstance = testClassInstance;
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            // copy from XunitTestRunner

            var output = string.Empty;

            TestOutputHelper testOutputHelper = null;
            foreach (object obj in ConstructorArguments)
            {
                testOutputHelper = obj as TestOutputHelper;
                if (testOutputHelper != null)
                    break;
            }

            if (testOutputHelper != null)
                testOutputHelper.Initialize(MessageBus, Test);

            var executionTime = await InvokeTestMethodAsync(aggregator);

            if (testOutputHelper != null)
            {
                output = testOutputHelper.Output;
                testOutputHelper.Uninitialize();
            }

            return Tuple.Create(executionTime, output);
        }

        private Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
            => new TestInvoker(_testClassInstance, Test, MessageBus, TestClass, TestMethod, aggregator, CancellationTokenSource).RunAsync();
    }
}