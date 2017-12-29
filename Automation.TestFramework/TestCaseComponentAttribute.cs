using System;

namespace Automation.TestFramework
{
    public abstract class TestCaseComponentAttribute : Attribute
    {
        public int Order { get; }

        protected TestCaseComponentAttribute(int order = 1, string description = null)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order), "Order must be positive");

            Order = order;
            Description = description;
        }

        public string Description { get; internal set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(Description)) throw new InvalidOperationException("Description is not set");
                return GetDisplayName(Description);
            }
        }

        protected abstract string GetDisplayName(string description);
    }
}