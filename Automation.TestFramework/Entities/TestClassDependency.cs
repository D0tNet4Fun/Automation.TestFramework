using Xunit.Abstractions;

namespace Automation.TestFramework.Entities
{
    internal class TestClassDependency
    {
        public DependencyType Type { get; set; }

        public ITypeInfo Class { get; set; }
    }

    internal enum DependencyType
    {
        Inheritance,
        Aggregation
    }
}