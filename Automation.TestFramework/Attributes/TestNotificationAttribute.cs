using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Used to decorate an assembly or a class to allow access to test execution events.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class TestNotificationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestNotificationAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the notification. This must implement <see cref="ITestNotification"/> and have a constructor with a single argument of type <c>object</c>.</param>
        public TestNotificationAttribute(Type type)
        {
            if (!type.IsClass && !typeof(ITestNotification).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type.Name} does not implement interface {nameof(ITestNotification)}.");

            var ctor = type.GetConstructor(new[] { typeof(object) });
            if (ctor == null)
                throw new ArgumentException($"Type {type.Name} does not have a constructor with a single argument of type {nameof(Object)}");

            Type = type;
        }

        /// <summary>
        /// Gets the type of the notification.
        /// </summary>
        public Type Type { get; }
    }
}