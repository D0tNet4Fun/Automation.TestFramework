namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the summary of a test case.
    /// </summary>
    public class SummaryAttribute : TestCaseComponentAttribute
    {
        public SummaryAttribute(string description)
            : base(0, description)
        {

        }

        protected override string GetDisplayName(string description)
            => description;
    }
}