namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a method that runs before a test case is executed.
    /// </summary>
    public class SetupAttribute : TestCaseComponentAttribute
    {
        public SetupAttribute(int order, string description = null)
            : base(order, description)
        {
        }

        protected override string GetDisplayName(string description)
            => $"[Setup] {Order} {description}";
    }
}