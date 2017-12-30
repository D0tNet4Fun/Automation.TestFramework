namespace Automation.TestFramework.Entities
{
    internal class TestStep
    {
        public ITest Input { get; set; }

        public ITest ExpectedResult { get; set; }
    }
}