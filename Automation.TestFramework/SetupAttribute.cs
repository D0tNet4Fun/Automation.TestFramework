namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a method that runs before a test case is executed.
    /// </summary>
    public class SetupAttribute : TestCaseComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which the target test method needs to be executed.</param>
        /// <param name="description">The user friendly description of the test method.</param>
        public SetupAttribute(int order = 1, string description = null)
            : base(order, description)
        {
        }

        /// <inheritdoc />
        protected override string GetDisplayName(string description)
            => $"[Setup] {Order}. {description}";
    }
}