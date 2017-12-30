using System;
using Automation.TestFramework;
using Automation.TestFramework.Tests;

[assembly: AssemblyFixture(typeof(AssemblyFixtureClass))]

namespace Automation.TestFramework.Tests
{
    public class AssemblyFixtureClass : IDisposable
    {
        public AssemblyFixtureClass()
        {
            // put a breakpoint and verify it is hit
        }

        public void Dispose()
        {
            // same
        }
    }
}