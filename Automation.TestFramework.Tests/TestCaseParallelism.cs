using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework.Tests
{
    public class TestCaseMultiBase
    {
        internal Worker1 Worker { get; }

        public TestCaseMultiBase(ITestOutputHelper testOutputHelper)
        {
            Worker = new Worker1(this, testOutputHelper);
        }

        internal class Worker1
        {
            private readonly object _owner;
            private readonly ITestOutputHelper _testOutputHelper;

            public Worker1(object owner, ITestOutputHelper testOutputHelper)
            {
                _owner = owner;
                _testOutputHelper = testOutputHelper;
            }

            public void DoWork()
            {
                _testOutputHelper.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Working for {_owner.GetType().Name}...");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                _testOutputHelper.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Work done for {_owner.GetType().Name}");
            }
        }
    }

    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SerialCollection : ICollectionFixture<SerialCollectionFixture>
    {
    }

    public class SerialCollectionFixture : IDisposable
    {
        public SerialCollectionFixture()
        {
        }

        public void Dispose()
        {
        }
    }

    [Collection("Serial")]
    public class SerialTCBase : TestCaseMultiBase
    {
        public SerialTCBase(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    [TestCase("TCS1")]
    [Trait("parallelism", "disabled")]
    public class SerialTC1 : SerialTCBase
    {
        public SerialTC1(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Summary]
        public void TCSerial_1()
        {
        }

        [Input]
        private void Input() => Worker.DoWork();
    }

    [TestCase("TCS2")]
    [Trait("parallelism", "disabled")]
    public class SerialTC2 : SerialTCBase
    {
        public SerialTC2(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Summary]
        public void TCSerial_2()
        {
        }

        [Input]
        private void Input() => Worker.DoWork();
    }



    [CollectionDefinition("Parallel1")]
    public class ParallelCollection1 : ICollectionFixture<ParallelCollectionFixture>
    {

    }

    public class ParallelCollectionFixture : IDisposable
    {
        public ParallelCollectionFixture()
        {
        }

        public void Dispose()
        {
        }
    }

    [Collection("Parallel1")]
    public class ParallelTestCase1Base : TestCaseMultiBase
    {
        public ParallelTestCase1Base(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    [TestCase("ParallelTC1_1")]
    [Trait("parallelism", "enabled(1)")]
    public class ParallelTestCase1_1 : ParallelTestCase1Base
    {
        public ParallelTestCase1_1(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Summary]
        public void TCParallel1_1()
        {

        }

        [Input]
        private void Input() => Worker.DoWork();
    }

    [TestCase("ParallelTC1_2")]
    [Trait("parallelism", "enabled(1)")]
    public class ParallelTestCase1_2 : ParallelTestCase1Base
    {
        public ParallelTestCase1_2(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Summary]
        public void TCParallel1_2()
        {
            throw new Exception("2");
        }

        [Input]
        private void Input() => Worker.DoWork();
    }
}