using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using ITest = Automation.TestFramework.Entities.ITest;

namespace Automation.TestFramework.Execution
{
    internal class TestInvoker : TestInvoker<ITestCase>
    {
        private readonly Type _testNotificationType;
        private readonly object _testClassInstance;
        private readonly TestStepContext _testStepContext;

        public TestInvoker(ITest test, IMessageBus messageBus, Type testClass, MethodInfo testMethod, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Type testNotificationType, TestStepContext testStepContext)
            : base(test, messageBus, testClass, new object[0], testMethod, new object[0], aggregator, cancellationTokenSource)
        {
            _testNotificationType = testNotificationType;
            _testClassInstance = test.Instance;
            _testStepContext = testStepContext;
        }

        protected override object CreateTestClass()
        {
            // we already have a test class instance; return null here, otherwise this would mean this class owns the instance and therefore it could dispose it later
            return null;
        }

        protected override Task BeforeTestMethodInvokedAsync()
        {
            _testStepContext?.Initialize();
            return base.BeforeTestMethodInvokedAsync();
        }

        protected override object CallTestMethod(object testClassInstance)
        {
            // ignore the input and use what we have
            return base.CallTestMethod(_testClassInstance);
        }

        private void TryNotify()
        {
            try
            {
                var notification = (ITestNotification)Activator.CreateInstance(_testNotificationType, _testClassInstance);
                var exception = Aggregator.ToException();
                notification.OnError(exception);
            }
            catch (Exception inner)
            {
                var error = $"Error in test notification of type {_testNotificationType.Name}: {inner}";
                // todo where to output this?
            }
        }

        protected override async Task AfterTestMethodInvokedAsync()
        {
            try
            {
                await base.AfterTestMethodInvokedAsync();

                if (_testNotificationType != null && Aggregator.HasExceptions)
                {
                    TryNotify();
                }
            }
            finally
            {
                _testStepContext?.Dispose();

                // clear the test class instance from the test, to avoid being serialized
                ((ITest)Test).Instance = null;
            }
        }
    }
}