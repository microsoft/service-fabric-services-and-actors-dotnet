// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Fabric.Health;
    using System.Linq;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Runtime;
    using FluentAssertions;
    using Moq;
    using Xunit;

    /// <summary>
    /// State manager tests.
    /// </summary>
    public class StatefulServiceLifeCycleTests
    {
        class RunAsyncBlockingCallTestService : StatefulBaseTestService
        {
            public RunAsyncBlockingCallTestService(StatefulServiceContext context)
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
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: RunAsyncBlockingCall()");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new RunAsyncBlockingCallTestService(serviceContext);
            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            partition.SetupGet(p => p.WriteStatus).Returns(PartitionAccessStatus.Granted);

            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            var changeRoleTask = testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None);

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.RunAsyncInvoked)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }

            Assert.True(changeRoleTask.IsCompleted && !changeRoleTask.IsCanceled && !changeRoleTask.IsFaulted);
            ((StatefulServiceReplicaAdapter) testServiceReplica).Test_IsRunAsyncTaskRunning().Should().BeTrue();
            
            testServiceReplica.CloseAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        [Fact]
        public void CancellationDuringWriteStatus()
        {
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: CancellationDuringWriteStatus()");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            IStatefulServiceReplica testServiceReplica =
                new StatefulServiceReplicaAdapter(serviceContext, new StatefulBaseTestService(serviceContext));

            var partition = new Mock<IStatefulServicePartition>();

            // This will make WaitForWriteStatusAsync to keep retrying in loop.
            partition.SetupGet(p => p.WriteStatus).Returns(PartitionAccessStatus.ReconfigurationPending);

            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();

            // This will throw if canceling during write status propagates out, as canceling of executeRunAsyncTask
            // (which includes waiting for write status) is  awaited during change role away from primary.
            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
        }

        class RunAsyncCancellationTestService : StatefulBaseTestService
        {
            private const int ToWait = 100;

            public bool StartedWaiting = false;

            public RunAsyncCancellationTestService(StatefulServiceContext context)
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
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: RunAsyncCancellation()");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new RunAsyncCancellationTestService(serviceContext);
            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            partition.SetupGet(p => p.WriteStatus).Returns(PartitionAccessStatus.Granted);
            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.StartedWaiting)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }

            // This will throw if cancellation propagates out, as canceling of RunAsync is awaited during 
            // change role away from primary.
            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
        }

        class RunAsyncSlowCancellationTestService : StatefulBaseTestService
        {
            public RunAsyncSlowCancellationTestService(StatefulServiceContext context)
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
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: RunAsyncSlowCancellation()");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new RunAsyncSlowCancellationTestService(serviceContext);
            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            partition.SetupGet(p => p.WriteStatus).Returns(PartitionAccessStatus.Granted);
            partition.Setup(p => p.ReportPartitionHealth(It.IsAny<HealthInformation>()));

            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();

            CancellationTokenSource source = new CancellationTokenSource(10000);
            while (!testService.RunAsyncInvoked)
            {
                Task.Delay(100, source.Token).GetAwaiter().GetResult();
            }
            
            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
            partition.Verify(p => p.ReportPartitionHealth(It.Is<HealthInformation>(hinfo => Utility.IsRunAsyncSlowCancellationHealthInformation(hinfo))), Times.AtLeastOnce);
        }

        class RunAsyncFailTestService : StatefulBaseTestService
        {
            public RunAsyncFailTestService(StatefulServiceContext context)
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
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: RunAsyncFail()");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, new RunAsyncFailTestService(serviceContext));

            var tcs = new TaskCompletionSource<bool>();
            var partition = new Mock<IStatefulServicePartition>();
            partition.SetupGet(p => p.WriteStatus).Returns(PartitionAccessStatus.Granted);
            partition.Setup(p => p.ReportFault(It.IsAny<FaultType>())).Callback(
                () =>
                {
                    tcs.SetResult(true);
                });

            partition.Setup(p => p.ReportPartitionHealth(It.IsAny<HealthInformation>()));

            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();

            Task.Run(
                () =>
                {
                    Task.Delay(10000).GetAwaiter().GetResult();
                    tcs.SetCanceled();
                });

            tcs.Task.GetAwaiter().GetResult().Should().BeTrue();

            // This will throw if RunAsync exception propagates out
            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();

            partition.Verify(p => p.ReportFault(It.IsNotIn(FaultType.Transient)), Times.Never());
            partition.Verify(p => p.ReportFault(FaultType.Transient), Times.Once());
            partition.Verify(p => p.ReportPartitionHealth(It.Is<HealthInformation>(hinfo => Utility.IsRunAsyncUnhandledExceptionHealthInformation(hinfo))), Times.Once());
        }
        
        [Fact]
        public void CommunicationListenerLifeCycle_P_S_P_N_NoListenOnSecondary()
        {
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: CommunicationListenerLifeCycle_P_S_P_N_NoListenOnSecondary");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new StatefulBaseTestService(serviceContext)
            {
                ListenOnSecondary = false
            };

            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 1;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened only once(U->P)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter)testServiceReplica).Test_CommunicationListeners.First().Should().Be(testService.CurrentListener.Object);

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Never());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 1;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened only once(U->P->S)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter) testServiceReplica).Test_CommunicationListeners.Should().BeNull();

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// S -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 2;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened twice(U->P->S->P)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter)testServiceReplica).Test_CommunicationListeners.First().Should().Be(testService.CurrentListener.Object);

                var firstListener = testService.Listeners.First();
                firstListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.Abort(), Times.Never());

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Never());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// P -> N");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.None, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 2;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened twice(U->P->S->P->N)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter) testServiceReplica).Test_CommunicationListeners.Should().BeNull();

                var firstListener = testService.Listeners.First();

                firstListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.Abort(), Times.Never());

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
        }

        [Fact]
        public void CommunicationListenerLifeCycle_P_S_P_N_ListenOnSecondary()
        {
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: CommunicationListenerLifeCycle_P_S_P_N_ListenOnSecondary");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new StatefulBaseTestService(serviceContext)
            {
                ListenOnSecondary = true
            };

            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            Console.WriteLine(@"// U -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 1;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened only once(U->P)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter)testServiceReplica).Test_CommunicationListeners.First().Should().Be(testService.CurrentListener.Object);

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Never());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// P -> S");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.ActiveSecondary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 2;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened twice(U->P->S)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter)testServiceReplica).Test_CommunicationListeners.First().Should().Be(testService.CurrentListener.Object);

                var firstListener = testService.Listeners[0];
                firstListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.Abort(), Times.Never());

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Never());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// S -> P");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 3;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened three times(U->P->S->P)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter)testServiceReplica).Test_CommunicationListeners.First().Should().Be(testService.CurrentListener.Object);

                var firstListener = testService.Listeners[0];
                firstListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.Abort(), Times.Never());

                var secondListener = testService.Listeners[1];
                secondListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                secondListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                secondListener.Verify(l => l.Abort(), Times.Never());

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Never());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            Console.WriteLine(@"// P -> N");
            testServiceReplica.ChangeRoleAsync(ReplicaRole.None, CancellationToken.None).GetAwaiter().GetResult();
            {
                const int expectedCount = 3;
                int actualCount = testService.Listeners.Count;
                actualCount.Should().Be(expectedCount, "listener has been opened three times(U->P->S->P->N)");
                testService.Listeners.Last().Should().Be(testService.CurrentListener);
                ((StatefulServiceReplicaAdapter) testServiceReplica).Test_CommunicationListeners.Should().BeNull();

                var firstListener = testService.Listeners[0];
                firstListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                firstListener.Verify(l => l.Abort(), Times.Never());

                var secondListener = testService.Listeners[1];
                secondListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                secondListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                secondListener.Verify(l => l.Abort(), Times.Never());

                testService.CurrentListener.Verify(l => l.OpenAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.CloseAsync(It.IsAny<CancellationToken>()), Times.Once());
                testService.CurrentListener.Verify(l => l.Abort(), Times.Never());
            }

            partition.Verify(p => p.ReportFault(It.IsAny<FaultType>()), Times.Never());
        }
        
        [Fact]
        public void ListenerExceptionOnAbort()
        {
            Console.WriteLine("StatefulServiceLifeCycleTests - Test Method: ListenerExceptionOnAbort");

            var serviceContext = TestMocksRepository.GetMockStatefulServiceContext();

            var testService = new StatefulBaseTestService(serviceContext)
            {
                EnableListenerExceptionOnAbort = true
            };

            IStatefulServiceReplica testServiceReplica = new StatefulServiceReplicaAdapter(serviceContext, testService);

            var partition = new Mock<IStatefulServicePartition>();
            testServiceReplica.OpenAsync(ReplicaOpenMode.New, partition.Object, CancellationToken.None).GetAwaiter().GetResult();

            testServiceReplica.ChangeRoleAsync(ReplicaRole.Primary, CancellationToken.None).GetAwaiter().GetResult();

            testServiceReplica.Abort();
        }
    }
}
