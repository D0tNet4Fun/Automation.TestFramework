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
        private readonly Type _testNotificationType;
        private readonly object _testClassInstance;

        public TestInvoker(ITest test, IMessageBus messageBus, Type testClass, MethodInfo testMethod, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Type testNotificationType)
            : base(test, messageBus, testClass, new object[0], testMethod, new object[0], aggregator, cancellationTokenSource)
        {
            _testNotificationType = testNotificationType;
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

            if (_testNotificationType == null)
                return TestMethod.Invoke(_testClassInstance, TestMethodArguments);

            try
            {
                return TestMethod.Invoke(_testClassInstance, TestMethodArguments);
            }
            catch (TargetInvocationException e)
            {
                try
                {
                    Notify(e.InnerException);
                }
                catch (Exception inner)
                {
                    var error = $"Error in test notification of type {_testNotificationType.Name}: {inner}";
                    // todo where to output this?
                }
                throw;
            }
        }

        private void Notify(Exception exception)
        {
            var notification = (ITestNotification)Activator.CreateInstance(_testNotificationType, _testClassInstance);
            notification.OnError(exception);
        }
    }
}