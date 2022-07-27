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
        /// Manual Migration base case
        /// </summary>
        /// <returns><see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ManualMigrationBaseCase()
        {
            var orchestrator = GetOrchestrator(MigrationMode.Manual);

            // ActorService calling Start Migration when the service is up
            await orchestrator.StartMigrationAsync(false, CancellationToken.None);

            var migrationTask = Task.Run(() => orchestrator.StartMigrationAsync(true, CancellationToken.None).ConfigureAwait(false));
            while (true)
            {
                var result = await orchestrator.GetResultAsync(CancellationToken.None);
                Assert.True(result.CurrentPhase <= MigrationPhase.Catchup);
                Assert.True(result.Status == MigrationState.InProgress);
                if (result.CurrentPhase == MigrationPhase.Catchup)
                {
                    await orchestrator.StartDowntimeAsync(true, CancellationToken.None);
                    break;
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            await migrationTask;
            var result1 = await orchestrator.GetResultAsync(CancellationToken.None);
            Assert.Equal(MigrationPhase.Completed, result1.CurrentPhase);
            Assert.Equal(MigrationState.Completed, result1.Status);
        }

        private static TargetMigrationOrchestrator GetOrchestrator(MigrationMode mode, MigrationPhase blockingPhase = MigrationPhase.None)
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
            mockFactory.Setup(o => o.StartMigrationAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .CallBase();
            mockFactory.Setup(o => o.StartDowntimeAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .CallBase();
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
            mockFactory.Setup(o => o.ValidateConfigForMigrationAsync(It.IsAny<CancellationToken>()));

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

            return phaseFactory.Object;
        }
    }
}
