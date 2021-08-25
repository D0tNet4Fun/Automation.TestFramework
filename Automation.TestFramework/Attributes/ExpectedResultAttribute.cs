namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the expected result of a test case step.
    /// </summary>
    public class ExpectedResultAttribute : TestCaseComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedResultAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which the target test method needs to be executed.</param>
        /// <param name="description">The user friendly description of the test method.</param>
        public ExpectedResultAttribute(int order = 1, string description = null)
            : base(order, description)
        {

        }

        /// <inheritdoc />
        protected override string GetDisplayName(string description)
            => $"[Expected result] {Order}. {description}";
    }
}