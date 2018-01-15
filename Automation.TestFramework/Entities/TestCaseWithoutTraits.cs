using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Entities
{
    internal class TestCaseWithoutTraits : LongLivedMarshalByRefObject, IXunitTestCase
    {
        private readonly IXunitTestCase _source;

        public TestCaseWithoutTraits(IXunitTestCase source)
        {
            _source = source;
            Traits = new Dictionary<string, List<string>>();
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            throw new NotSupportedException();
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            _source.Serialize(info);
        }

        public string DisplayName => _source.DisplayName;

        public string SkipReason => _source.SkipReason;

        public ISourceInformation SourceInformation
        {
            get => _source.SourceInformation;
            set => _source.SourceInformation = value;
        }

        public ITestMethod TestMethod => _source.TestMethod;

        public object[] TestMethodArguments => _source.TestMethodArguments;

        public string UniqueID => _source.UniqueID;

        public Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            => _source.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);

        public IMethodInfo Method => _source.Method;

        public Dictionary<string, List<string>> Traits { get; }
    }
}