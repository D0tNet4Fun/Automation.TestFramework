using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework
{
    public class Framework : XunitTestFramework
    {
        public Framework(IMessageSink messageSink)
            : base(messageSink)
        {
            messageSink.OnMessage(new DiagnosticMessage("Hello world"));
        }
    }
}
