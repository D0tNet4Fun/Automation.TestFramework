namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the expected result of a test case step.
    /// </summary>
    public class ExpectedResultAttribute : TestCaseComponentAttribute
    {
        public ExpectedResultAttribute(int order, string description)
            : base(order, description)
        {

        }

        protected override string GetDisplayName(string description)
            => $"[Expected result] {Order}. {description}";
    }
}