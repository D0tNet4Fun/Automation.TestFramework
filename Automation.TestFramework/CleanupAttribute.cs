namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a method that runs after a test case is executed, regardless the outcome.
    /// </summary>
    public class CleanupAttribute : TestCaseComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanupAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which the target test method needs to be executed.</param>
        /// <param name="description">The user friendly description of the test method.</param>
        public CleanupAttribute(int order = 1, string description = null)
            : base(order, description)
        {
        }

        /// <inheritdoc />
        protected override string GetDisplayName(string description)
            => $"[Cleanup] {Order}. {description}";
    }
}