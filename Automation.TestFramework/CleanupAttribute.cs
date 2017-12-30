namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a method that runs after a test case is executed, regardless the outcome.
    /// </summary>
    public class CleanupAttribute : TestCaseComponentAttribute
    {
        public CleanupAttribute(int order = 1, string description = null)
            : base(order, description)
        {
        }

        protected override string GetDisplayName(string description)
            => $"[Cleanup] {Order}. {description}";
    }
}