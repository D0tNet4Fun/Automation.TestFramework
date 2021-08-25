namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as a precondition of a test case.
    /// </summary>
    public class PreconditionAttribute : TestCaseComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreconditionAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which the target test method needs to be executed.</param>
        /// <param name="description">The user friendly description of the test method.</param>
        public PreconditionAttribute(int order = 1, string description = null)
            : base(order, description)
        {

        }

        /// <inheritdoc />
        protected override string GetDisplayName(string description)
            => $"[Precondition] {Order}. {description}";
    }
}