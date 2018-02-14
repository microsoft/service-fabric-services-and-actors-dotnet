// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime.Volatile
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using ActorStateTable = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateTable<
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType,
        string,
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData>;
    using ActorStateDataWrapper = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateTable<
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType,
        string,
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData>.ActorStateDataWrapper;
    using ActorStateType = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType;
    using SecondaryPump = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.SecondaryPump;
    using CopyStateEnumerator = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.CopyStateEnumerator;
    using Moq;
  
    using Xunit;

#pragma warning disable xUnit1024
    public class SecondaryPumpTests : VolatileStateProviderTestBase
    {
        [Fact]
        public void TestCopyAndReplication()
        {
            this.TestCopyAndReplication(GetStatesPerReplication());
            this.TestCopyAndReplication(GetStatesPerReplication(4));
        }

        [Fact]
        public void TestCopyUpToSequenceNumber()
        {
            this.TestCopyUpToSequenceNumber(GetStatesPerReplication());
            this.TestCopyUpToSequenceNumber(GetStatesPerReplication(4));
        }

        [Fact]
        public void TestCopyAndReplicationDelete()
        {
            this.TestCopyAndReplicationDelete(GetStatesPerReplication());
            this.TestCopyAndReplicationDelete(GetStatesPerReplication(4));
        }

        [Fact]
        public void TestReplication()
        {
            this.TestReplication(GetStatesPerReplication());
            this.TestReplication(GetStatesPerReplication(4));
        }

        private void TestCopyAndReplication(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("### TestCopyAndReplication ###");

            TestCase("### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            var maxReplicationMessageSize = 2048;
            var maxCopyMessageSize = maxReplicationMessageSize / 2;

            var copyOrReplicationSerializer = VolatileActorStateProvider.CreateCopyOrReplicationOperationSerializer();

            var dataLength = (maxCopyMessageSize / (3 * statesPerReplication[ActorStateType.Actor])) + 1;
            var primaryStateTable = new ActorStateTable();
            var copyKeyPrefixs = new string[] { "a", "b", "c", "d", "e" };
            var primarySequenceNumber = 0;

            var replicationUnitBatch = new List<ReplicationUnit>();
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in copyKeyPrefixs)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in copyKeyPrefixs)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var copyStateEnumerator = new CopyStateEnumerator(
                primaryStateTable.GetShallowCopiesEnumerator(primarySequenceNumber),
                copyOrReplicationSerializer,
                primarySequenceNumber,
                maxCopyMessageSize);

            var replicationKeyPrefixes = new string[] { "w", "x", "y", "z" };

            replicationUnitBatch = new List<ReplicationUnit>();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            var replicationData = TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var secondaryStateTable = new ActorStateTable();

            var replicationStreamReadySignal = new TaskCompletionSource<object>();
            var stateReplicator = new MockStateReplicator(
                copyOrReplicationSerializer,
                copyStateEnumerator,
                replicationData,
                replicationStreamReadySignal.Task);

            var partition = new Mock<IStatefulServicePartition>();
            var secondaryPump = new SecondaryPump(
                partition.Object,
                secondaryStateTable,
                stateReplicator,
                copyOrReplicationSerializer,
                new VolatileLogicalTimeManager(new MockSnapshotHandler(), TimeSpan.MaxValue),
                "SecondaryPumpUnitTest");

            TestCase("# TestCopyAndReplication: Testcase 1: Pump copy and replication operations");

            secondaryPump.StartCopyAndReplicationPump();

            var pumpCompletionTask = secondaryPump.WaitForPumpCompletionAsync();

            // Wait for copy pump to drain copy stream
            Thread.Sleep(TimeSpan.FromSeconds(5));

            FailTestIf(
                pumpCompletionTask.IsCompleted,
                "Pump CopyAndReplicationTask completed before replication stream is ready.");

            foreach (var keyPrefix in copyKeyPrefixs)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    copyKeyPrefixs.Length,
                    copyKeyPrefixs.Length,
                    copyKeyPrefixs.Length * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Signal replication stream to be ready.");
            replicationStreamReadySignal.SetResult(null);

            pumpCompletionTask.Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# TestCopyAndReplication: Testcase 2: Pump replication operations");

            secondaryPump.StartReplicationPump();
            secondaryPump.WaitForPumpCompletionAsync().Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        private void TestCopyUpToSequenceNumber(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("### TestCopyUpToSequenceNumber ###");

            TestCase("### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            var maxReplicationMessageSize = 2048;
            var maxCopyMessageSize = maxReplicationMessageSize / 2;

            var copyOrReplicationSerializer = VolatileActorStateProvider.CreateCopyOrReplicationOperationSerializer();

            var dataLength = (maxCopyMessageSize / (3 * statesPerReplication[ActorStateType.Actor])) + 1;
            var primaryStateTable = new ActorStateTable();
            var copyKeyPrefixes = new string[] { "a", "b", "c", "d", "e" };
            var primarySequenceNumber = 0;

            var replicationUnitBatch = new List<ReplicationUnit>();
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var replicationKeyPrefixes = new string[] { "w", "x", "y", "z" };

            replicationUnitBatch = new List<ReplicationUnit>();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var upToSequenceNumber = copyKeyPrefixes.Length + replicationKeyPrefixes.Length / 2;
            var copyStateEnumerator = new CopyStateEnumerator(
                primaryStateTable.GetShallowCopiesEnumerator(upToSequenceNumber),
                copyOrReplicationSerializer,
                upToSequenceNumber,
                maxCopyMessageSize);

            var secondaryStateTable = new ActorStateTable();

            var replicationStreamReadySignal = new TaskCompletionSource<object>();
            var stateReplicator = new MockStateReplicator(
                copyOrReplicationSerializer,
                copyStateEnumerator,
                new List<List<ActorStateDataWrapper>>(),
                replicationStreamReadySignal.Task);

            var partition = new Mock<IStatefulServicePartition>();
            var secondaryPump = new SecondaryPump(
                partition.Object,
                secondaryStateTable,
                stateReplicator,
                copyOrReplicationSerializer,
                new VolatileLogicalTimeManager(new MockSnapshotHandler(), TimeSpan.MaxValue),
                "SecondaryPumpUnitTest");

            TestCase("# TestCopyUpToSequenceNumber: Testcase 1: Pump copy operations");

            secondaryPump.StartCopyAndReplicationPump();

            var pumpCompletionTask = secondaryPump.WaitForPumpCompletionAsync();

            Thread.Sleep(TimeSpan.FromSeconds(5));

            FailTestIf(
                pumpCompletionTask.IsCompleted,
                "Pump CopyAndReplicationTask completed before replication stream is ready.");

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    upToSequenceNumber,
                    upToSequenceNumber,
                    upToSequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Signal replication stream to be ready.");
            replicationStreamReadySignal.SetResult(null);

            pumpCompletionTask.Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    (keyPrefix == "w" || keyPrefix == "x") ? true : false,
                    dataLength,
                    upToSequenceNumber,
                    upToSequenceNumber,
                    upToSequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# TestCopyUpToSequenceNumber: Testcase 2: Pump replication operations (none)");

            secondaryPump.StartReplicationPump();
            secondaryPump.WaitForPumpCompletionAsync().Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    (keyPrefix == "w" || keyPrefix == "x") ? true : false,
                    dataLength,
                    upToSequenceNumber,
                    upToSequenceNumber,
                    upToSequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        private void TestCopyAndReplicationDelete(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("### TestCopyAndReplicationDelete ###");

            TestCase("### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            var maxReplicationMessageSize = 2048;
            var maxCopyMessageSize = maxReplicationMessageSize / 2;

            var copyOrReplicationSerializer = VolatileActorStateProvider.CreateCopyOrReplicationOperationSerializer();

            var dataLength = (maxCopyMessageSize / (3 * statesPerReplication[ActorStateType.Actor])) + 1;
            var primaryStateTable = new ActorStateTable();
            var copyKeyPrefixes = new string[] { "a", "b", "c", "d", "e" };
            var primarySequenceNumber = 0;

            var replicationUnitBatch = new List<ReplicationUnit>();
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var deletedCopyKeyPrefixes = new string[] { "a", "c", "e" };
            foreach (var keyPrefix in deletedCopyKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(primaryStateTable, replicationUnit);
                TestCommitUpdate(primaryStateTable, primarySequenceNumber);
            }

            var copyStateEnumerator = new CopyStateEnumerator(
                primaryStateTable.GetShallowCopiesEnumerator(primarySequenceNumber),
                copyOrReplicationSerializer,
                primarySequenceNumber,
                maxCopyMessageSize);

            var replicationKeyPrefixes = new string[] { "v", "w", "x", "y", "z" };

            replicationUnitBatch = new List<ReplicationUnit>();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            var replicationData = TestApplyBatch(primaryStateTable, replicationUnitBatch);

            long expectedCount =
                (copyKeyPrefixes.Length
                - deletedCopyKeyPrefixes.Length +
                replicationKeyPrefixes.Length) *
                statesPerReplication[ActorStateType.Actor];

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    expectedCount);
            }

            var deletedReplicationKeyPrefixes = new string[] { "v", "x", "z" };
            foreach (var keyPrefix in deletedReplicationKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(primaryStateTable, replicationUnit);
                TestCommitUpdate(primaryStateTable, primarySequenceNumber);

                replicationUnit.UpdateSequenceNumber();
                replicationData.Add(replicationUnit.ActorStateDataWrapperList);
            }

            var secondaryStateTable = new ActorStateTable();

            var replicationStreamReadySignal = new TaskCompletionSource<object>();
            var stateReplicator = new MockStateReplicator(
                copyOrReplicationSerializer,
                copyStateEnumerator,
                replicationData,
                replicationStreamReadySignal.Task);

            var partition = new Mock<IStatefulServicePartition>();
            var secondaryPump = new SecondaryPump(
                partition.Object,
                secondaryStateTable,
                stateReplicator,
                copyOrReplicationSerializer,
                new VolatileLogicalTimeManager(new MockSnapshotHandler(), TimeSpan.MaxValue),
                "SecondaryPumpUnitTest");

            TestCase("# TestCopyAndReplicationDelete: Testcase 1: Pump copy and replication operations");

            secondaryPump.StartCopyAndReplicationPump();

            var pumpCompletionTask = secondaryPump.WaitForPumpCompletionAsync();

            // Wait for copy pump to drain copy stream
            Thread.Sleep(TimeSpan.FromSeconds(5));

            FailTestIf(
                pumpCompletionTask.IsCompleted,
                "pumpCompletionTask.IsCompleted. Expected: false Actual: {0}.", pumpCompletionTask.IsCompleted);

            foreach (var keyPrefix in copyKeyPrefixes)
            {
                var expectedExists = !deletedCopyKeyPrefixes.ToList().Exists(o => (o == keyPrefix));

                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    expectedExists,
                    dataLength,
                    copyKeyPrefixes.Length + deletedCopyKeyPrefixes.Length,
                    copyKeyPrefixes.Length + deletedCopyKeyPrefixes.Length,
                    (copyKeyPrefixes.Length - deletedCopyKeyPrefixes.Length) * statesPerReplication[ActorStateType.Actor] + 1);
            }

            TestCase("# Signal replication stream to be ready.");
            replicationStreamReadySignal.SetResult(null);

            pumpCompletionTask.Wait();

            expectedCount =
                (copyKeyPrefixes.Length
                + replicationKeyPrefixes.Length
                - deletedCopyKeyPrefixes.Length
                - deletedReplicationKeyPrefixes.Length)
                * statesPerReplication[ActorStateType.Actor] + 1;

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var expectedExists = !deletedReplicationKeyPrefixes.ToList().Exists(o => (o == keyPrefix));

                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    expectedExists,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    expectedCount);
            }

            TestCase("# TestCopyAndReplicationDelete: Testcase 2: Pump replication operations");

            secondaryPump.StartReplicationPump();
            secondaryPump.WaitForPumpCompletionAsync().Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var expectedExists = !deletedReplicationKeyPrefixes.ToList().Exists(o => (o == keyPrefix));

                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    expectedExists,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    expectedCount);
            }

            TestCase("# Passed");
        }

        private void TestReplication(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("### TestReplication ###");

            TestCase("### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            var maxReplicationMessageSize = 2048;
            var maxCopyMessageSize = maxReplicationMessageSize / 2;

            var copyOrReplicationSerializer = VolatileActorStateProvider.CreateCopyOrReplicationOperationSerializer();

            var dataLength = (maxCopyMessageSize / (3 * statesPerReplication[ActorStateType.Actor])) + 1;
            var primaryStateTable = new ActorStateTable();
            var copyKeyPrefixs = new string[] { "a", "b", "c", "d", "e" };
            var primarySequenceNumber = 0;

            var replicationUnitBatch = new List<ReplicationUnit>();
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in copyKeyPrefixs)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in copyKeyPrefixs)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var copyStateEnumerator = new CopyStateEnumerator(
                primaryStateTable.GetShallowCopiesEnumerator(primarySequenceNumber),
                copyOrReplicationSerializer,
                primarySequenceNumber,
                maxCopyMessageSize);

            var replicationKeyPrefixes = new string[] { "w", "x", "y", "z" };

            replicationUnitBatch = new List<ReplicationUnit>();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    ++primarySequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    dataLength);

                replicationUnitBatch.Add(replicationUnit);
                replicationUnitDict[keyPrefix] = replicationUnit;
            }

            var replicationData = TestApplyBatch(primaryStateTable, replicationUnitBatch);

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    primaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    primarySequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var secondaryStateTable = new ActorStateTable();
            var stateReplicator = new MockStateReplicator(
                copyOrReplicationSerializer,
                copyStateEnumerator,
                replicationData,
                Task.FromResult(true));

            var partition = new Mock<IStatefulServicePartition>();
            var secondaryPump = new SecondaryPump(
                partition.Object,
                secondaryStateTable,
                stateReplicator,
                copyOrReplicationSerializer,
                new VolatileLogicalTimeManager(new MockSnapshotHandler(), TimeSpan.MaxValue),
                "SecondaryPumpUnitTest");

            TestCase("# TestReplication: Testcase 1: Pump replication operations");

            secondaryPump.StartReplicationPump();
            secondaryPump.WaitForPumpCompletionAsync().Wait();

            foreach (var keyPrefix in replicationKeyPrefixes)
            {
                VerifyReads(
                    secondaryStateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    dataLength,
                    primarySequenceNumber,
                    primarySequenceNumber,
                    replicationKeyPrefixes.Length * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        #region Helper inner classes

        private class MockStateReplicator : IStateReplicator
        {
            private readonly MockCopyOperationStream _copyStream;
            private readonly MockReplicationOperationStream _replicationStream;

            public MockStateReplicator(
                DataContractSerializer replicationSerializer,
                IOperationDataStream copyStateEnumerator,
                List<List<ActorStateDataWrapper>> replicationList,
                Task replicationStreamReadySignal)
            {
                this._copyStream = new MockCopyOperationStream(copyStateEnumerator);
                this._replicationStream = new MockReplicationOperationStream(
                    replicationSerializer,
                    replicationList,
                    replicationStreamReadySignal);
            }

            IOperationStream IStateReplicator.GetCopyStream()
            {
                return this._copyStream;
            }

            IOperationStream IStateReplicator.GetReplicationStream()
            {
                return this._replicationStream;
            }

            Task<long> IStateReplicator.ReplicateAsync(OperationData operationData, System.Threading.CancellationToken cancellationToken, out long sequenceNumber)
            {
                throw new NotImplementedException("ReplicateAsync");
            }

            void IStateReplicator.UpdateReplicatorSettings(ReplicatorSettings settings)
            {
                throw new NotImplementedException("UpdateReplicatorSettings");
            }
        }

        private class MockCopyOperationStream : IOperationStream
        {
            private readonly IOperationDataStream _copyStateEnumerator;
            private long _sequenceNumber;

            public MockCopyOperationStream(IOperationDataStream copyStateEnumerator)
            {
                this._copyStateEnumerator = copyStateEnumerator;
                this._sequenceNumber = 0;
            }

            Task<IOperation> IOperationStream.GetOperationAsync(System.Threading.CancellationToken cancellationToken)
            {
                var operationData = this._copyStateEnumerator.GetNextAsync(cancellationToken).Result;

                // Secondary pump should take sequence number off each operation data itself
                var copySequenceNumber = ++this._sequenceNumber;

                return CreateCompletedTask<IOperation>(
                    operationData == null ? null : new MockOperation(operationData, copySequenceNumber));
            }
        }

        private class MockReplicationOperationStream : IOperationStream
        {
            private readonly DataContractSerializer _replicationSerializer;
            private readonly List<List<ActorStateDataWrapper>> _replicationData;
            private readonly Task _replicationStreamReadySignal;
            private int _replicationDataIndex;

            public MockReplicationOperationStream(
                DataContractSerializer replicationSerializer,
                List<List<ActorStateTable.ActorStateDataWrapper>> replicationData,
                Task replicationStreamReadySignal)
            {
                this._replicationSerializer = replicationSerializer;
                this._replicationData = replicationData;
                this._replicationStreamReadySignal = replicationStreamReadySignal;
                this._replicationDataIndex = 0;
            }

            async Task<IOperation> IOperationStream.GetOperationAsync(System.Threading.CancellationToken cancellationToken)
            {
                await this._replicationStreamReadySignal;

                if (this._replicationDataIndex < this._replicationData.Count)
                {
                    var actorStateDataWrapperList = this._replicationData[this._replicationDataIndex++];
                    var replicationSequenceNumber = actorStateDataWrapperList[0].SequenceNumber;
                    var replicationOperation = new VolatileActorStateProvider.CopyOrReplicationOperation(actorStateDataWrapperList);

                    // Secondary pump should take sequence number off the replication operation
                    // rather than the data
                    //
                    foreach (var actorStateDataWrapper in actorStateDataWrapperList)
                    {
                        actorStateDataWrapper.UpdateSequenceNumber(0);
                    }

                    return new MockOperation(
                        VolatileActorStateProvider.SerializeToOperationData(
                            this._replicationSerializer,
                            replicationOperation),
                        replicationSequenceNumber);
                }

                return null;
            }
        }

        private class MockOperation : IOperation
        {
            private readonly OperationData _operationData;
            private readonly long _sequenceNumber;

            public MockOperation(
                OperationData operationData,
                long sequenceNumber)
            {
                this._operationData = operationData;
                this._sequenceNumber = sequenceNumber;
            }

            void IOperation.Acknowledge()
            {
                // no-op
            }

            long IOperation.AtomicGroupId
            {
                get { throw new NotImplementedException("AtomicGroupId"); }
            }

            OperationData IOperation.Data
            {
                get { return this._operationData; }
            }

            OperationType IOperation.OperationType
            {
                get { throw new NotImplementedException("OperationType"); }
            }

            long IOperation.SequenceNumber
            {
                get { return this._sequenceNumber; }
            }
        }

        private class MockSnapshotHandler : VolatileLogicalTimeManager.ISnapshotHandler
        {
            Task VolatileLogicalTimeManager.ISnapshotHandler.OnSnapshotAsync(TimeSpan currentLogicalTime)
            {
                TestLog("OnSnapshotAsync({0})", currentLogicalTime);
                return CreateCompletedTask<object>(null);
            }
        }

        private static Task<T> CreateCompletedTask<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        #endregion Helper inner classes
    }
}
