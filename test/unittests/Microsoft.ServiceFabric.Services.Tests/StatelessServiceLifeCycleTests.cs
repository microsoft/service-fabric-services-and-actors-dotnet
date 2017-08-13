// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Runtime;
    using FluentAssertions;
    using Moq;
    using Xunit;
    using Communication.Runtime;

    public class StatelessServiceLifeCycleTests
    {
        class RunAsyncBlockingCallTestService : StatelessService
        {
            public RunAsyncBlockingCallTestService(StatelessServiceContext context)
                : base(context)
            {
                this.RunAsyncInvoked = false;
            }

            public bool RunAsyncInvoked { get; private set; }

            protected override Task RunAsync(CancellationToken cancellationToken)
            {
                this.RunAsyncInvoked = true;

                long i = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    i++;
                }

                return Task.FromResult(i);
            }
        }

        [Fact]
        public void RunAsyncBlockingCall()
        {
            Console.WriteLine("StatelessServiceLifeCycleTests - Test Method: RunAsyncBlockingCall()");

            var serviceContext = TestMocksRepository.GetMockStatelessServiceContext();

            var testService = new RunAsyncBlockingCallTestService(serviceContext);
            IStatelessServiceInstance testServiceReplica = new StatelessServiceInstanceAdapter(serviceContext, testService);

            var partition = new Mock<IStatelessServicePartition>();

            var openTask = testServiceReplica.OpenAsync(partition.Object, CancellationToken.None);

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.RunAsyncInvoked)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }

            Assert.True(openTask.IsCompleted && !openTask.IsCanceled && !openTask.IsFaulted);
            ((StatelessServiceInstanceAdapter)testServiceReplica).Test_IsRunAsyncTaskRunning().Should().BeTrue();

            testServiceReplica.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        class RunAsyncCancellationTestService : StatelessService
        {
            private const int ToWait = 100;

            public bool StartedWaiting = false;

            public RunAsyncCancellationTestService(StatelessServiceContext context)
                : base(context)
            {
            }

            protected override async Task RunAsync(CancellationToken cancellationToken)
            {
                this.StartedWaiting = true;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(ToWait, cancellationToken);
                }
            }
        }

        [Fact]
        public void RunAsyncCancellation()
        {
            Console.WriteLine("StatelessServiceLifeCycleTests - Test Method: RunAsyncCancellation()");

            var serviceContext = TestMocksRepository.GetMockStatelessServiceContext();

            var testService = new RunAsyncCancellationTestService(serviceContext);
            IStatelessServiceInstance testServiceReplica = new StatelessServiceInstanceAdapter(serviceContext, testService);

            var partition = new Mock<IStatelessServicePartition>();
            testServiceReplica.OpenAsync(partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.StartedWaiting)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }

            // This will throw if cancellation propagates out, as canceling of RunAsync is 
            // awaited during close of stateless serice instance.
            testServiceReplica.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
        }

        class RunAsyncSlowCancellationTestService : StatelessService
        {
            public RunAsyncSlowCancellationTestService(StatelessServiceContext context)
                : base(context)
            {
                this.RunAsyncInvoked = false;
            }

            public bool RunAsyncInvoked { get; private set; }

            protected override async Task RunAsync(CancellationToken cancellationToken)
            {
                this.RunAsyncInvoked = true;
                await Task.Delay(TimeSpan.FromSeconds(30), CancellationToken.None);
            }
        }

        [Fact]
        public void RunAsyncSlowCancellation()
        {
            Console.WriteLine("StatelessServiceLifeCycleTests - Test Method: RunAsyncSlowCancellation()");

            var serviceContext = TestMocksRepository.GetMockStatelessServiceContext();

            var testService = new RunAsyncSlowCancellationTestService(serviceContext);
            IStatelessServiceInstance testServiceReplica = new StatelessServiceInstanceAdapter(serviceContext, testService);

            var partition = new Mock<IStatelessServicePartition>();
            partition.Setup(p => p.ReportPartitionHealth(It.IsAny<HealthInformation>()));

            testServiceReplica.OpenAsync(partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.RunAsyncInvoked)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }

            testServiceReplica.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
            partition.Verify(p => p.ReportPartitionHealth(It.Is<HealthInformation>(hinfo => Utility.IsRunAsyncSlowCancellationHealthInformation(hinfo))), Times.AtLeastOnce);
        }

        class RunAsyncFailTestService : StatelessService
        {
            public RunAsyncFailTestService(StatelessServiceContext context)
                : base(context)
            {
            }

            protected override Task RunAsync(CancellationToken cancellationToken)
            {
                return Task.Run(() => { throw new FabricException(); }, CancellationToken.None);
            }
        }

        [Fact]
        public void RunAsyncFail()
        {
            Console.WriteLine("StatelessServiceLifeCycleTests - Test Method: RunAsyncFail()");

            var serviceContext = TestMocksRepository.GetMockStatelessServiceContext();

            IStatelessServiceInstance testServiceReplica = new StatelessServiceInstanceAdapter(serviceContext, new RunAsyncFailTestService(serviceContext));

            var tcs = new TaskCompletionSource<bool>();
            var partition = new Mock<IStatelessServicePartition>();
            partition.Setup(p => p.ReportFault(It.IsAny<FaultType>())).Callback(
                () =>
                {
                    tcs.SetResult(true);
                });

            partition.Setup(p => p.ReportPartitionHealth(It.IsAny<HealthInformation>()));

            testServiceReplica.OpenAsync(partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Task.Run(
                () =>
                {
                    Task.Delay(10000).GetAwaiter().GetResult();
                    tcs.SetCanceled();
                });

            tcs.Task.GetAwaiter().GetResult().Should().BeTrue();

            // This will throw if RunAsync exception propagates out
            testServiceReplica.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsNotIn(FaultType.Transient)), Times.Never());
            partition.Verify(p => p.ReportFault(FaultType.Transient), Times.Once());
            partition.Verify(p => p.ReportPartitionHealth(It.Is<HealthInformation>(hinfo => Utility.IsRunAsyncUnhandledExceptionHealthInformation(hinfo))), Times.Once());
        }

        class ListenerExceptionOnAbortService : StatelessService
        {
            public ListenerExceptionOnAbortService(StatelessServiceContext context)
                : base(context)
            {
            }

            protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            {
                return new[]
                {
                    new ServiceInstanceListener(this.CreateCommunicationListener),
                };
            }

            private ICommunicationListener CreateCommunicationListener(ServiceContext context)
            {
                Console.WriteLine("Creating listener");
                var mockListener = new Mock<ICommunicationListener>();
                mockListener.Setup(x => x.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult("Address"));
                mockListener.Setup(x => x.Abort()).Throws(new Exception("Listener Abort exception."));

                return mockListener.Object;
            }
        }

        [Fact]
        public void ListenerExceptionOnAbort()
        {
            Console.WriteLine("StatelessServiceLifeCycleTests - Test Method: RunAsyncFail()");

            var serviceContext = TestMocksRepository.GetMockStatelessServiceContext();

            IStatelessServiceInstance testServiceInstance =
                new StatelessServiceInstanceAdapter(serviceContext, new ListenerExceptionOnAbortService(serviceContext));

            var partition = new Mock<IStatelessServicePartition>();

            testServiceInstance.OpenAsync(partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            // This will throw if listener exception propagates out
            testServiceInstance.Abort();
        }
    }
}
