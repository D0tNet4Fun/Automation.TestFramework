using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Allows defining a fixture class to execute code before all tests in the assembly start.
    /// If cleanup is needed then the class should implement <see cref="IDisposable"/> and the test framework will call <c>Dispose</c> after all tests in the assembly finish.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyFixtureAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFixtureAttribute"/> class specifying the type of the fixture.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture. This must be a class.</param>
        public AssemblyFixtureAttribute(Type fixtureType)
        {
            if (!fixtureType.IsClass) throw new ArgumentException("The fixture type must be a class", nameof(fixtureType));
            FixtureType = fixtureType;
        }

        /// <summary>
        /// Gets the type of the fixture.
        /// </summary>
        public Type FixtureType { get; }
    }
}