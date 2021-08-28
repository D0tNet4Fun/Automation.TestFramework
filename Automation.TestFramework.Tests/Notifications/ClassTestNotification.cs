using System;

namespace Automation.TestFramework.Tests.Notifications
{
    public class ClassTestNotification : ITestNotification
    {
        public ClassTestNotification(object testClassInstance)
        {
            Instance = this;
            TestClassInstance = testClassInstance;
        }

        [ThreadStatic] 
        private static ClassTestNotification _instance;

        public static ClassTestNotification Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public object TestClassInstance { get; set; }
        public Exception Error { get; set; }

        public void OnError(Exception error) => Error = error;
    }
}