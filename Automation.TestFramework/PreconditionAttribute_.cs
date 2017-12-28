namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as a precondition of a test case.
    /// </summary>
    public class PreconditionAttribute : TestCaseComponentAttribute
    {
        public PreconditionAttribute(int order, string description)
            : base(order, description)
        {

        }

        protected override string GetDisplayName(string description)
            => $"[Precondition] {Order}. {description}";
    }
}