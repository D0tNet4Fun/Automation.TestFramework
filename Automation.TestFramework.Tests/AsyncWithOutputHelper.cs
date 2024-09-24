using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Automation.TestFramework.Tests
{
    [TestCase(nameof(AsyncWithOutputHelper))]
    public class AsyncWithOutputHelper(ITestOutputHelper testOutputHelper)
    {
        [Summary]
        public void Summary()
        {

        }

        [Input]
        private Task Input1()
        {
            testOutputHelper.WriteLine(nameof(Input1));
            return Task.CompletedTask;
        }

        [Input(2)]
        private Task Input2()
        {
            testOutputHelper.WriteLine(nameof(Input2));
            return Task.CompletedTask;
        }
    }
}
