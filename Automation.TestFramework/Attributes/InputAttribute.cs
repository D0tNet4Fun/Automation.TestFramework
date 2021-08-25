namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the input of a test case step.
    /// </summary>
    public class InputAttribute : TestCaseComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which the target test method needs to be executed.</param>
        /// <param name="description">The user friendly description of the test method.</param>
        public InputAttribute(int order = 1, string description = null)
            : base(order, description)
        {

        }

        /// <inheritdoc />
        protected override string GetDisplayName(string description)
            => $"[Input] {Order}. {description}";
    }
}