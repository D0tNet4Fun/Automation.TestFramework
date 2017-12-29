namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the input of a test case step.
    /// </summary>
    public class InputAttribute : TestCaseComponentAttribute
    {
        public InputAttribute(int order = 1, string description = null)
            : base(order, description)
        {

        }

        protected override string GetDisplayName(string description)
            => $"[Input] {Order}. {description}";
    }
}