using System;
using System.Reflection;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;
using ITest = Automation.TestFramework.Entities.ITest;

namespace Automation.TestFramework.Execution
{
    internal class TestInvoker : TestInvoker<ITestCase>
    {
        private readonly object _testClassInstance;

        public TestInvoker(ITest test, IMessageBus messageBus, Type testClass, MethodInfo testMethod, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, new object[0], testMethod, new object[0], aggregator, cancellationTokenSource)
        {
            _testClassInstance = test.TestClassInstance;
        }

        protected override object CreateTestClass()
        {
            // we already have a test class instance; return null here, otherwise this would mean this class owns the instance and therefore it could dispose it later
            return null;
        }

        protected override object CallTestMethod(object testClassInstance)
        {
            // ignore the input and use what we have
            return TestMethod.Invoke(_testClassInstance, TestMethodArguments);
        }
    }
}