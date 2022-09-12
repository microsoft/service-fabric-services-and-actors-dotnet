// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes;
    using Xunit;

    /// <summary>
    /// Migration Worker Tests
    /// </summary>
    public class MigrationWorkerTests
    {
        /// <summary>
        /// Worker test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task StartWorkTest()
        {
            var worker = new MigrationWorker(
                new MockKVStoRCMigrationActorStateProvider(new MockReliableCollectionsStateProvider()),
                new Runtime.ActorTypeInformation(),
                new MockServicePartitionClient(null, new Uri("fabric:/Blah/BlahBlah")),
                new MigrationSettings
                {
                    ChunksPerEnumeration = 10,
                    KeyValuePairsPerChunk = 100,
                    SourceServiceUri = new Uri("fabric:/blah/blahblah"),
                },
                new PhaseInput.WorkerInput
                {
                    StartSeqNum = 0,
                    EndSeqNum = 100,
                    Iteration = 1,
                    WorkerId = 1,
                    Phase = Migration.MigrationPhase.Copy,
                    Status = Migration.MigrationState.InProgress,
                    StartDateTimeUTC = DateTime.UtcNow,
                },
                "TestTrace");
            await this.PersistInputAsync(worker);
            var result = await worker.StartWorkAsync(CancellationToken.None);

            Assert.NotEqual(default(DateTime), result.StartDateTimeUTC);
            Assert.NotEqual(default(DateTime), result.EndDateTimeUTC);
            Assert.Equal(result.StartSeqNum, worker.Input.StartSeqNum);
            Assert.Equal(result.EndSeqNum, worker.Input.EndSeqNum);
            Assert.Equal(MockServicePartitionClient.EnumerationSize, result.LastAppliedSeqNum);
            Assert.Equal(result.Phase, worker.Input.Phase);
            Assert.Equal(Migration.MigrationState.Completed, result.Status);
        }

        private async Task PersistInputAsync(MigrationWorker worker)
        {
            var metaDict = await worker.StateProvider.GetMetadataDictionaryAsync();
            await metaDict.AddAsync(
                worker.StateProvider.GetStateManager().CreateTransaction(),
                MigrationConstants.Key(MigrationConstants.PhaseWorkerStartDateTimeUTC, worker.Input.Phase, worker.Input.Iteration, worker.Input.WorkerId),
                worker.Input.StartDateTimeUTC.ToString());
            await metaDict.AddAsync(
                worker.StateProvider.GetStateManager().CreateTransaction(),
                MigrationConstants.Key(MigrationConstants.PhaseWorkerStartSeqNum, worker.Input.Phase, worker.Input.Iteration, worker.Input.WorkerId),
                worker.Input.StartSeqNum.ToString());
            await metaDict.AddAsync(
                worker.StateProvider.GetStateManager().CreateTransaction(),
                MigrationConstants.Key(MigrationConstants.PhaseWorkerEndSeqNum, worker.Input.Phase, worker.Input.Iteration, worker.Input.WorkerId),
                worker.Input.EndSeqNum.ToString());
            await metaDict.AddAsync(
                worker.StateProvider.GetStateManager().CreateTransaction(),
                MigrationConstants.Key(MigrationConstants.PhaseWorkerCurrentStatus, worker.Input.Phase, worker.Input.Iteration, worker.Input.WorkerId),
                worker.Input.Status.ToString());
        }
    }
}
