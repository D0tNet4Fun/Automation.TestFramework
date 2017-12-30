using System;

namespace Automation.TestFramework
{
    public abstract class TestCaseComponentAttribute : Attribute
    {
        public int Order { get; }
        private readonly string _description;

        protected TestCaseComponentAttribute(int order, string description)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order), "Order must be positive");

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));

            Order = order;
            _description = description;
        }

        public string DisplayName
        {
            get => GetDisplayName(_description);
            set => throw new NotSupportedException("Use ctor to set description"); // todo
        }

        protected abstract string GetDisplayName(string description);
    }
}