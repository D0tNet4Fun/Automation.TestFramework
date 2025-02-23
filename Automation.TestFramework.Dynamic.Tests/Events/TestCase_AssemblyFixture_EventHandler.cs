using System;
using Automation.TestFramework.Dynamic.Tests.Events;
using Xunit;

[assembly: AssemblyFixture(typeof(TestCase_AssemblyFixture_EventHandler.Fixture))]

namespace Automation.TestFramework.Dynamic.Tests.Events;

public class TestCase_AssemblyFixture_EventHandler : IDisposable
{
    public void Dispose()
    {
        Assert.NotNull(Fixture.Exception);
        Assert.Same(this, Fixture.Sender);
        Assert.Same(Exception, Fixture.Exception);
    }
    
    public Exception Exception { get; } = new("error");

    [Summary("Test case with failed input raises OnError event in class fixture")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "This is the input that fails", () => throw Exception);
    }

    private void Input()
    {
        Assert.Fail();
    }

    public class Fixture : IDisposable
    {
        public static object Sender { get; private set; }
        public static Exception Exception { get; private set; }
        
        public Fixture()
        {
            EventSource.Instance.StepError += OnStepError;
        }

        public void Dispose()
        {
            EventSource.Instance.StepError -= OnStepError;
        }

        private void OnStepError(object sender, Exception e)
        {
            Sender = sender;
            Exception = e;
        }
    }
}