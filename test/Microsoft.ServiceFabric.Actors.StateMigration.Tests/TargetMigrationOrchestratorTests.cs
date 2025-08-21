// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.Migration;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
    using Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes;
    using Microsoft.ServiceFabric.Services.Communication.Client;
    using Moq;
    using Xunit;

    /// <summary>
    /// Target Migration orchestrator tests.
    /// </summary>
    public class TargetMigrationOrchestratorTests
    {
        /// <summary>
        /// Auto Migration base case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AutoMigrationBaseCase()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Auto);
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            var result = await orchestrator.GetResultAsync(CancellationToken.None);

            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);
        }

        /// <summary>
        /// Auto Migration failover case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AutoMigrationFailoverCase()
        {
            // Failover in Copy phase
            var orchestrator = GetOrchestrator(MigrationMode.Auto);
            var metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Copy.ToString(), (_, __) => MigrationPhase.Copy.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            var result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);

            // Failover in catchup phase
            orchestrator = GetOrchestrator(MigrationMode.Auto);
            metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Catchup.ToString(), (_, __) => MigrationPhase.Catchup.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);

            // Failover in downtime phase
            orchestrator = GetOrchestrator(MigrationMode.Auto);
            metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Downtime.ToString(), (_, __) => MigrationPhase.Downtime.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);
        }

        /// <summary>
        /// Auto Migration abort case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AutoMigrationAbortCase()
        {
            var tcs = new CancellationTokenSource();
            var orchestrator = GetOrchestrator(MigrationMode.Auto, MigrationPhase.Copy);
            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(false, tcs.Token).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                await orchestrator.AbortMigrationAsync(true, CancellationToken.None);
                break;
            }

            try
            {
                await migrationTask;
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }

            var result = await orchestrator.GetResultAsync(CancellationToken.None);

            Assert.Equal(MigrationState.Aborted, result.Status);
        }

        /// <summary>
        /// Manual Migration base case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ManualMigrationBaseCase()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Manual);

            // ActorService calling Start Migration when the service is up
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);

            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (InvalidMigrationOperationException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                if (result1.CurrentPhase == MigrationPhase.Catchup)
                {
                    await orchestrator.StartDowntimeAsync(true, CancellationToken.None);
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            await migrationTask;
            var result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);
        }

        /// <summary>
        /// Manual Migration failover case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ManualMigrationFailoverCase()
        {
            // Failover in Copy phase
            var orchestrator = GetOrchestrator(MigrationMode.Manual);
            var metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Copy.ToString(), (_, __) => MigrationPhase.Copy.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                if (result1.CurrentPhase == MigrationPhase.Catchup)
                {
                    await orchestrator.StartDowntimeAsync(true, CancellationToken.None);
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            await migrationTask;
            var result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);

            // Failover in catchup phase
            orchestrator = GetOrchestrator(MigrationMode.Manual);
            metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Catchup.ToString(), (_, __) => MigrationPhase.Catchup.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                if (result1.CurrentPhase == MigrationPhase.Catchup)
                {
                    await orchestrator.StartDowntimeAsync(true, CancellationToken.None);
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            await migrationTask;
            result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);

            // Failover in downtime phase
            orchestrator = GetOrchestrator(MigrationMode.Manual);
            metaDict = await ((KVStoRCMigrationActorStateProvider)orchestrator.GetMigrationActorStateProvider()).GetMetadataDictionaryAsync();
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentPhase, MigrationPhase.Downtime.ToString(), (_, __) => MigrationPhase.Downtime.ToString());
            await metaDict.AddOrUpdateAsync(orchestrator.Transaction, MigrationConstants.MigrationCurrentStatus, MigrationState.InProgress.ToString(), (_, __) => MigrationState.InProgress.ToString());
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            result = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result.Status);
        }

        /// <summary>
        /// Manual Migration abort case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ManualMigrationAbortCase()
        {
            var tcs = new CancellationTokenSource();
            var orchestrator = GetOrchestrator(MigrationMode.Manual);
            await orchestrator.StartMigrationAsync(false, tcs.Token);
            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, tcs.Token).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                await orchestrator.AbortMigrationAsync(true, CancellationToken.None);
                break;
            }

            try
            {
                await migrationTask;
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }

            var result = await orchestrator.GetResultAsync(CancellationToken.None);

            Assert.Equal(MigrationState.Aborted, result.Status);
        }

        /// <summary>
        /// Check if actor calls allowed.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task AreActorCallsAllowedTest()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Auto);
            Assert.False(orchestrator.AreActorCallsAllowed());
            bool callbackInvoked = false;
            orchestrator.RegisterCompletionCallback(
                (_, __) =>
                {
                    callbackInvoked = true;
                    return Task.CompletedTask;
                });

            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            Assert.True(orchestrator.AreActorCallsAllowed());
            Assert.True(callbackInvoked);

            // Abort case
            var tcs = new CancellationTokenSource();
            orchestrator = GetOrchestrator(MigrationMode.Manual);
            Assert.False(orchestrator.AreActorCallsAllowed());
            callbackInvoked = false;
            orchestrator.RegisterCompletionCallback(
                (_, __) =>
                {
                    callbackInvoked = true;
                    return Task.CompletedTask;
                });

            await orchestrator.StartMigrationAsync(false, tcs.Token);
            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, tcs.Token).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                Assert.True(result1.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result1.Status == MigrationState.InProgress);
                await orchestrator.AbortMigrationAsync(true, CancellationToken.None);
                break;
            }

            try
            {
                await migrationTask;
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }

            Assert.False(orchestrator.AreActorCallsAllowed());
            Assert.True(callbackInvoked);
        }

        /// <summary>
        /// Actor call forwarding test.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task IsActorCallToBeForwardedTest()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Manual);

            // Migration not initialized. Hence except exception.
            Assert.Throws<MigrationFrameworkNotInitializedException>(() => orchestrator.IsActorCallToBeForwarded());

            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                // Migration in progress. Call to be forwarded.
                Assert.True(orchestrator.IsActorCallToBeForwarded());
                await orchestrator.StartDowntimeAsync(true, CancellationToken.None);
                break;
            }

            // Migration complete. Do not forward request.
            await migrationTask;
            Assert.False(orchestrator.IsActorCallToBeForwarded());

            var tcs = new CancellationTokenSource();
            orchestrator = GetOrchestrator(MigrationMode.Manual);
            await orchestrator.StartMigrationAsync(false, tcs.Token);
            migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, tcs.Token).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                // Migration in progress. Call to be forwarded.
                Assert.True(orchestrator.IsActorCallToBeForwarded());
                await orchestrator.AbortMigrationAsync(true, CancellationToken.None);
                break;
            }

            try
            {
                await migrationTask;
            }
            catch (OperationCanceledException)
            {
                // Do nothing
            }

            // Migration aborted call to be forwarded.
            Assert.True(orchestrator.IsActorCallToBeForwarded());

            orchestrator = GetOrchestrator(MigrationMode.Auto, MigrationPhase.Downtime);
            migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(false, tcs.Token).ConfigureAwait(false).GetAwaiter().GetResult());
            while (true)
            {
                MigrationResult result1 = null;
                try
                {
                    result1 = await orchestrator.GetResultAsync(CancellationToken.None);
                }
                catch (MigrationFrameworkNotInitializedException e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.None || result1.Status == MigrationState.None)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    continue;
                }

                if (result1.CurrentPhase == MigrationPhase.Downtime)
                {
                    // Downtime phase. Call should not be forwarded
                    Assert.False(orchestrator.IsActorCallToBeForwarded());
                    break;
                }
            }
        }

        /// <summary>
        /// Invalid operations test.
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task InvalidOperationsTest()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Auto);
            await Assert.ThrowsAsync<MigrationFrameworkNotInitializedException>(() => orchestrator.AbortMigrationAsync(true, CancellationToken.None));
            await Assert.ThrowsAsync<MigrationFrameworkNotInitializedException>(() => orchestrator.StartDowntimeAsync(true, CancellationToken.None));
            await Assert.ThrowsAsync<MigrationFrameworkNotInitializedException>(() => orchestrator.GetResultAsync(CancellationToken.None));
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);
            await Assert.ThrowsAsync<InvalidMigrationOperationException>(() => orchestrator.AbortMigrationAsync(true, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidMigrationOperationException>(() => orchestrator.StartDowntimeAsync(true, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidMigrationOperationException>(() => orchestrator.StartMigrationAsync(true, CancellationToken.None));

            orchestrator = GetOrchestrator(MigrationMode.Auto, invalidConfig: true);
            await Assert.ThrowsAsync<InvalidMigrationConfigException>(() => orchestrator.StartMigrationAsync(false, CancellationToken.None));

            orchestrator = GetOrchestrator(MigrationMode.Auto);
            await Assert.ThrowsAsync<InvalidMigrationOperationException>(() => orchestrator.StartMigrationAsync(true, CancellationToken.None));
        }

        private static TargetMigrationOrchestrator GetOrchestrator(MigrationMode mode, MigrationPhase blockingPhase = MigrationPhase.None, bool invalidConfig = false)
        {
            var setting = new KVSToRCMigration.MigrationSettings
            {
                MigrationMode = mode,
                CopyPhaseParallelism = 2,
                ChunksPerEnumeration = 10,
                KeyValuePairsPerChunk = 20,
                SourceServiceUri = new Uri("fabric:/blah/blahblah"),
                DowntimeThreshold = 1000,
            };

            var mockSp = new MockKVStoRCMigrationActorStateProvider(new MockReliableCollectionsStateProvider());
            var mockFactory = new Mock<TargetMigrationOrchestrator>(
                    mockSp,
                    new ActorTypeInformation(),
                    new StatefulServiceContext(
                        new NodeContext("TestNode", new NodeId(new BigInteger(0), new BigInteger(0)), new BigInteger(0), "TestNodeType", "10.10.10.10"),
                        new Mock<ICodePackageActivationContext>().Object,
                        "TestServiceType",
                        new Uri("fabric:/blah/blahblah"),
                        null,
                        Guid.NewGuid(),
                        0L),
                    setting,
                    GetMockExFilter(setting),
                    new MockServicePartitionClient(null, new Uri("fabric:/Blah/BlahBlah")),
                    "TestTraceId");
            mockFactory.CallBase = true;
            mockFactory.Setup(o => o.GetMigrationPhaseWorkload(It.IsAny<MigrationPhase>(), It.IsAny<int>()))
                .Returns<MigrationPhase, int>((phase, __) =>
                {
                    if (phase != MigrationPhase.Completed)
                    {
                        var res = GetMockPhaseWorkload(phase, blockingPhase != MigrationPhase.None && blockingPhase == phase);
                        var metaDict = mockSp.GetMetadataDictionaryAsync().GetAwaiter().GetResult();
                        metaDict.AddOrUpdateAsync(mockSp.GetStateManager().CreateTransaction(), MigrationConstants.MigrationCurrentPhase, phase.ToString(), (_, __) => phase.ToString()).GetAwaiter().GetResult();
                        return res;
                    }

                    return null;
                });
            mockFactory.Setup(o => o.ValidateConfigForMigrationAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(_ =>
                {
                    if (invalidConfig)
                    {
                        throw new InvalidMigrationConfigException();
                    }

                    return Task.CompletedTask;
                });

            return mockFactory.Object;
        }

        private static PartitionHealthExceptionFilter GetMockExFilter(KVSToRCMigration.MigrationSettings settings)
        {
            var exFilterFactory = new Mock<PartitionHealthExceptionFilter>(settings);
            exFilterFactory.CallBase = false;
            bool abortMigration = false;
            bool rethrow = true;
            exFilterFactory.Setup(ex => ex.ReportPartitionHealthIfNeeded(It.IsAny<Exception>(), It.IsAny<IStatefulServicePartition>(), out abortMigration, out rethrow));
            return exFilterFactory.Object;
        }

        private static IMigrationPhaseWorkload GetMockPhaseWorkload(MigrationPhase currentPhase, bool blockingCall = false)
        {
            var phaseFactory = new Mock<IMigrationPhaseWorkload>();
            phaseFactory.Setup(pw => pw.StartOrResumeMigrationAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(token =>
                {
                    token.ThrowIfCancellationRequested();
                    if (blockingCall)
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }
                    }

                    var result = new PhaseResult
                    {
                        Status = MigrationState.Completed,
                        Phase = currentPhase,
                        StartDateTimeUTC = DateTime.UtcNow,
                        EndDateTimeUTC = DateTime.UtcNow,
                        StartSeqNum = 0,
                        EndSeqNum = 0,
                        LastAppliedSeqNum = 0,
                        Iteration = 1,
                        NoOfKeysMigrated = 0,
                        WorkerCount = 1,
                    };

                    return Task.FromResult(result);
                });

            phaseFactory.SetupGet(pw => pw.Phase).Returns(currentPhase);

            return phaseFactory.Object;
        }
    }
}
