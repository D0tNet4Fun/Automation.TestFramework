using System;
using Automation.TestFramework;
using Automation.TestFramework.Tests.Notifications;

[assembly: TestNotification(typeof(GlobalTestNotification))]


namespace Automation.TestFramework.Tests.Notifications
{
    public class GlobalTestNotification : ITestNotification
    {
        public GlobalTestNotification(object testClassInstance)
        {
            Instance = this;
            TestClassInstance = testClassInstance;
        }

        [ThreadStatic]
        private static GlobalTestNotification _instance;

        public static GlobalTestNotification Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public object TestClassInstance { get; set; }
        public Exception Error { get; set; }

        public void OnError(Exception error) => Error = error;
    }
}