// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime.Volatile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using ActorStateTable = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateTable<
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType,
        string,
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData>;
    using ActorStateType = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType;

#pragma warning disable xUnit1024
    /// <summary>
    /// Contains tests for VolatileActorStateTable.
    /// </summary>
    public class VolatileActorStateTableTests : VolatileStateProviderTestBase
    {
        /// <summary>
        /// Tests PrepareCommit.
        /// </summary>
        [Fact]
        public void TestPrepareCommit()
        {
            this.TestPrepareCommitInternal(GetStatesPerReplication());
            this.TestPrepareCommitInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// Tests enumerating MaxSequenceNumber.
        /// </summary>
        [Fact]
        public void TestEnumerateMaxSequenceNumber()
        {
            this.TestEnumerateMaxSequenceNumberInternal(GetStatesPerReplication());
            this.TestEnumerateMaxSequenceNumberInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// Tests Apply.
        /// </summary>
        [Fact]
        public void TestApply()
        {
            this.TestApplyInternal(GetStatesPerReplication());
            this.TestApplyInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// TestUpdateApply
        /// </summary>
        [Fact]
        public void TestUpdateApply()
        {
            this.TestUpdateApplyInternal(GetStatesPerReplication());
            this.TestUpdateApplyInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// TestUpdateCommit
        /// </summary>
        [Fact]
        public void TestUpdateCommit()
        {
            this.TestUpdateCommitInternal(GetStatesPerReplication());
            this.TestUpdateCommitInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// TestSnapshot
        /// </summary>
        [Fact]
        public void TestSnapshot()
        {
            this.TestSnapshotInternal(GetStatesPerReplication());
            this.TestSnapshotInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// TestSnapshotScale
        /// </summary>
        [Fact]
        public void TestSnapshotScale()
        {
            TestCase("#########################");
            TestCase("### TestSnapshotScale ###");
            TestCase("#########################");

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();

            var targetReplicationCount = 1 * 1000;
            var statesPerReplication = GetStatesPerReplication(10);

            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            TestLog("Generating {0} keys...", targetReplicationCount * statesPerReplication[ActorStateType.Actor]);

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (var ix = 0; ix < targetReplicationCount; ++ix)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    ix.ToString(),
                    statesPerReplication[ActorStateType.Actor],
                    1);

                TestApply(stateTable, replicationUnit, false);
            }

            stopwatch.Stop();

            TestLog(
                "Generated {0} keys in {1}",
                targetReplicationCount * statesPerReplication[ActorStateType.Actor],
                stopwatch.Elapsed);

            stopwatch.Restart();

            var snapshot = stateTable.GetShallowCopiesEnumerator(long.MaxValue);

            stopwatch.Stop();

            TestLog(
                "Snapshot {0} keys in {1}: committed={2} uncommitted={3}",
                targetReplicationCount * statesPerReplication[ActorStateType.Actor],
                stopwatch.Elapsed,
                snapshot.CommittedCount,
                snapshot.UncommittedCount);

            TestCase("# Passed");
        }

        /// <summary>
        /// TestMultipleTypes
        /// </summary>
        [Fact]
        public void TestMultipleTypes()
        {
            this.TestMultipleTypesInternal(GetStatesPerReplication());
            this.TestMultipleTypesInternal(GetStatesPerReplication(3));
        }

        /// <summary>
        /// TestDelete
        /// </summary>
        [Fact]
        public void TestDelete()
        {
            this.TestDeleteInternal(GetStatesPerReplication());
            this.TestDeleteInternal(GetStatesPerReplication(3));
        }

        private void TestPrepareCommitInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("#########################");
            TestCase("### TestPrepareCommit ###");
            TestCase("#########################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            TestCase("# Testcase 1: In order prepare, commit, prepare, commit ...");

            foreach (var keyPrefix in new string[] { "a", "b", "c" })
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);

                TestCommitUpdate(stateTable, sequenceNumber);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Testcase 2: In order prepare, prepare, ... commit, commit ...");

            var keyPrefixList = new string[] { "d", "e", "f" };
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            var commitSequenceNumber = sequenceNumber;

            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber * 2);

                replicationUnitDict.Add(keyPrefix, replicationUnit);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    commitSequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            foreach (var keyPrefix in keyPrefixList)
            {
                ++commitSequenceNumber;

                TestCommitUpdate(stateTable, commitSequenceNumber);

                VerifyReads(
                    stateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    commitSequenceNumber * 2,
                    commitSequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Testcase 3: Out of order commits");

            keyPrefixList = new string[] { "g", "h", "i" };
            replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            var preCommitSequenceNumber = sequenceNumber;
            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    (long)keyPrefix.ToCharArray()[0]);

                replicationUnitDict.Add(keyPrefix, replicationUnit);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    preCommitSequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var commitSequenceNumber1 = sequenceNumber;
            Task.Factory.StartNew(() => { TestCommitUpdate(stateTable, commitSequenceNumber1); });
            Thread.Sleep(500);
            VerifyReads(
                stateTable,
                replicationUnitDict["i"],
                statesPerReplication,
                false,
                0,
                preCommitSequenceNumber,
                sequenceNumber,
                sequenceNumber * statesPerReplication[ActorStateType.Actor]);

            var commitSequenceNumber2 = commitSequenceNumber1 - 1;
            Task.Factory.StartNew(() => { TestCommitUpdate(stateTable, commitSequenceNumber2); });
            Thread.Sleep(500);
            VerifyReads(
                stateTable,
                replicationUnitDict["h"],
                statesPerReplication,
                false,
                0,
                preCommitSequenceNumber,
                sequenceNumber,
                sequenceNumber * statesPerReplication[ActorStateType.Actor]);

            var commitSequenceNumber3 = commitSequenceNumber2 - 1;
            TestCommitUpdate(stateTable, commitSequenceNumber3);
            Thread.Sleep(500);

            foreach (var keyPrefix in keyPrefixList)
            {
                VerifyReads(
                    stateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    (long)keyPrefix[0],
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        private void TestEnumerateMaxSequenceNumberInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("######################################");
            TestCase("### TestEnumerateMaxSequenceNumber ###");
            TestCase("######################################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            TestCase("# Testcase 1: Commmitted values only");

            var committedKeyPrefixList = new string[] { "apple", "orange", "banana", };
            foreach (var keyPrefix in committedKeyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);

                TestCommitUpdate(stateTable, sequenceNumber);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                long.MaxValue,
                sequenceNumber,
                sequenceNumber,
                sequenceNumber * statesPerReplication[ActorStateType.Actor]);

            TestCase("# Testcase 2: Commmitted + uncommitted values");

            var uncommittedKeyPrefixList = new string[] { "grape", "pear", "kiwi" };
            foreach (var keyPrefix in uncommittedKeyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length);

                TestPrepareUpdate(stateTable, replicationUnit);

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    committedKeyPrefixList.Length,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                long.MaxValue,
                sequenceNumber - uncommittedKeyPrefixList.Length,
                sequenceNumber,
                sequenceNumber * statesPerReplication[ActorStateType.Actor]);

            TestCase("# Passed");
        }

        private void TestApplyInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("#################");
            TestCase("### TestApply ###");
            TestCase("#################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            TestCase("# Testcase 1: Singleton apply");

            var keyPrefixList = new string[] { "f", "fo", "foo" };
            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length);

                TestApply(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Testcase 2: Batch apply");

            var keyPrefixBatchList = new string[][]
            {
                new string[] { "b", "ba", "barr" },
                new string[] { "x", "xy", "xyz" },
                new string[] { "a", "ab", "abc" },
            };

            foreach (var keyPrefixBatch in keyPrefixBatchList)
            {
                var replicationUnitBatch = new List<ReplicationUnit>();
                var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

                foreach (var keyPrefix in keyPrefixBatch)
                {
                    ++sequenceNumber;

                    var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                        sequenceNumber,
                        keyPrefix,
                        statesPerReplication[ActorStateType.Actor],
                        keyPrefix.Length);

                    replicationUnitBatch.Add(replicationUnit);
                    replicationUnitDict.Add(keyPrefix, replicationUnit);
                }

                TestApplyBatch(stateTable, replicationUnitBatch);

                foreach (var keyPrefix in keyPrefixBatch)
                {
                    VerifyReads(
                        stateTable,
                        replicationUnitDict[keyPrefix],
                        statesPerReplication,
                        true,
                        keyPrefix.Length,
                        sequenceNumber,
                        sequenceNumber,
                        sequenceNumber * statesPerReplication[ActorStateType.Actor]);
                }
            }

            TestCase("# Passed");
        }

        private void TestUpdateApplyInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("#######################");
            TestCase("### TestUpdateApply ###");
            TestCase("#######################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            var keyPrefixList = new string[] { "a-apply", "b-apply", "c-apply" };
            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length);

                TestApply(stateTable, replicationUnit);

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var commitSequenceNumber = sequenceNumber;
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length * 2);

                replicationUnitDict.Add(keyPrefix, replicationUnit);

                TestPrepareUpdate(stateTable, replicationUnit);

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    keyPrefixList.Length,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            foreach (var keyPrefix in keyPrefixList)
            {
                ++commitSequenceNumber;

                TestCommitUpdate(stateTable, commitSequenceNumber);

                VerifyReads(
                    stateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    keyPrefix.Length * 2,
                    commitSequenceNumber,
                    sequenceNumber,
                    (sequenceNumber + (keyPrefixList.Length - commitSequenceNumber)) * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        private void TestUpdateCommitInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("########################");
            TestCase("### TestUpdateCommit ###");
            TestCase("########################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            var keyPrefixList = new string[] { "a-commit", "b-commit", "c-commit" };
            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);

                TestCommitUpdate(stateTable, sequenceNumber);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var commitSequenceNumber = sequenceNumber;
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var keyPrefix in keyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    keyPrefix.Length * 2);

                replicationUnitDict.Add(keyPrefix, replicationUnit);

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    keyPrefix.Length,
                    keyPrefixList.Length,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            foreach (var keyPrefix in keyPrefixList)
            {
                ++commitSequenceNumber;

                TestCommitUpdate(stateTable, commitSequenceNumber);

                VerifyReads(
                    stateTable,
                    replicationUnitDict[keyPrefix],
                    statesPerReplication,
                    true,
                    keyPrefix.Length * 2,
                    commitSequenceNumber,
                    sequenceNumber,
                    (sequenceNumber + (keyPrefixList.Length - commitSequenceNumber)) * statesPerReplication[ActorStateType.Actor]);
            }

            TestCase("# Passed");
        }

        private void TestSnapshotInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("####################");
            TestCase("### TestSnapshot ###");
            TestCase("####################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            var committedKeyPrefixList = new string[] { "w", "x", "y", "z" };
            var replicationUnitDict = new Dictionary<string, ReplicationUnit>();

            foreach (var commitedkeyPrefix in committedKeyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    commitedkeyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    commitedkeyPrefix.Length);

                replicationUnitDict[commitedkeyPrefix] = replicationUnit;

                TestApply(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    commitedkeyPrefix.Length,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var updatedKeyPrefixList = new string[] { "w", "y" };
            foreach (var updatedkeyPrefix in updatedKeyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    updatedkeyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    updatedkeyPrefix.Length * 2);

                replicationUnitDict[updatedkeyPrefix] = replicationUnit;

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    updatedkeyPrefix.Length,
                    committedKeyPrefixList.Length,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var uncommittedKeyPrefixList = new string[] { "a", "b", "c" };
            foreach (var uncommittedkeyPrefix in uncommittedKeyPrefixList)
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    uncommittedkeyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    uncommittedkeyPrefix.Length * 2);

                replicationUnitDict[uncommittedkeyPrefix] = replicationUnit;

                TestPrepareUpdate(stateTable, replicationUnit);
                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    committedKeyPrefixList.Length,
                    sequenceNumber,
                    sequenceNumber * statesPerReplication[ActorStateType.Actor]);
            }

            var committedSnapshot = stateTable.GetShallowCopiesEnumerator(committedKeyPrefixList.Length);
            var knownSnapshot = stateTable.GetShallowCopiesEnumerator(long.MaxValue);

            var updateSequenceNumber = committedKeyPrefixList.Length;

            foreach (var key in updatedKeyPrefixList)
            {
                ++updateSequenceNumber;
                TestCommitUpdate(stateTable, updateSequenceNumber);
            }

            TestCommitUpdate(stateTable, ++updateSequenceNumber);

            var expectedCount = (committedKeyPrefixList.Length +
                                 uncommittedKeyPrefixList.Length) *
                                 statesPerReplication[ActorStateType.Actor];

            var keyPrefixes = new string[] { "x", "z", "w", "y", "a", "b", "c" };
            var expectedResults = new bool[] { true, true, true, true, true, false, false };
            var expectedLengths = new int[] { 1, 1, 2, 2, 2, 0, 0 };

            for (var i = 0; i < keyPrefixes.Length; i++)
            {
                VerifyReads(
                    stateTable,
                    replicationUnitDict[keyPrefixes[i]],
                    statesPerReplication,
                    expectedResults[i],
                    expectedLengths[i],
                    updateSequenceNumber,
                    sequenceNumber,
                    expectedCount);
            }

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                committedSnapshot,
                updateSequenceNumber,
                sequenceNumber,
                committedKeyPrefixList.Length * statesPerReplication[ActorStateType.Actor]);

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                knownSnapshot,
                updateSequenceNumber,
                sequenceNumber,
                sequenceNumber * statesPerReplication[ActorStateType.Actor]);

            TestCase("# Passed");
        }

        private void TestMultipleTypesInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("#######################################################");
            TestCase("### TestMultipleTypes ###");
            TestCase("#######################################################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            var committedEntriesCount = 0;
            var uncommittedEntriesCount = 0;

            TestCase("# Testcase 1: In order prepare, commit, prepare, commit ...");
            {
                ++sequenceNumber;

                var keyPrefix = "a";

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "L";
                var timestamp = TimeSpan.FromSeconds(sequenceNumber);

                var replicationUnit = ReplicationUnit.CreateForUpdateTimeStamp(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.LogicalTimestamp],
                    timestamp);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    timestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];
                committedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    timestamp,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "rem";
                var reminderName = "Rem-Name1";

                var replicationUnit = ReplicationUnit.CreateForUpdateReminder(
                   sequenceNumber,
                   key,
                   statesPerReplication[ActorStateType.Reminder],
                   reminderName);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    reminderName,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Reminder];
                committedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    reminderName,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            TestCase("# Testcase 2: Duplicate keys per type ...");

            var expectedCount = sequenceNumber;
            {
                ++sequenceNumber;

                var keyPrefix = "a";

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber - expectedCount,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "L";
                var oldTimestamp = TimeSpan.FromSeconds(sequenceNumber - expectedCount);
                var timestamp = TimeSpan.FromSeconds(sequenceNumber);

                var replicationUnit = ReplicationUnit.CreateForUpdateTimeStamp(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.LogicalTimestamp],
                    timestamp);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    oldTimestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    timestamp,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "rem";
                var oldReminderName = "Rem-Name1";
                var reminderName = "Rem-Name2";

                var replicationUnit = ReplicationUnit.CreateForUpdateReminder(
                   sequenceNumber,
                   key,
                   statesPerReplication[ActorStateType.Reminder],
                   reminderName);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    oldReminderName,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    reminderName,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            TestCase("# Testcase 3: Duplicate keys across types ...");

            var duplicateKey = "Dupe";
            {
                ++sequenceNumber;

                var keyPrefix = duplicateKey;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = duplicateKey;
                var timestamp = TimeSpan.FromSeconds(sequenceNumber);

                var replicationUnit = ReplicationUnit.CreateForUpdateTimeStamp(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.LogicalTimestamp],
                    timestamp);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    timestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];
                committedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    timestamp,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = duplicateKey;
                var reminderName = "Rem-Name3";

                var replicationUnit = ReplicationUnit.CreateForUpdateReminder(
                   sequenceNumber,
                   key,
                   statesPerReplication[ActorStateType.Reminder],
                   reminderName);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    reminderName,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Reminder];
                committedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    reminderName,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            TestCase("# Testcase 4: Enumeration by type ...");

            var baseCountPerType = 2;

            var baseCountActorType = baseCountPerType;
            foreach (var keyPrefix in new string[] { "x", "y" })
            {
                ++sequenceNumber;
                ++baseCountActorType;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    sequenceNumber,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                stateTable.GetShallowCopiesEnumerator(ActorStateType.Actor),
                sequenceNumber,
                sequenceNumber,
                baseCountActorType * statesPerReplication[ActorStateType.Actor]);
            {
                ++sequenceNumber;

                var key = "MyTimestamp";
                var timestamp = TimeSpan.FromSeconds(sequenceNumber);

                var replicationUnit = ReplicationUnit.CreateForUpdateTimeStamp(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.LogicalTimestamp],
                    timestamp);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    timestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];
                committedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                   stateTable,
                   replicationUnit,
                   statesPerReplication,
                   true,
                   timestamp,
                   sequenceNumber,
                   sequenceNumber,
                   uncommittedEntriesCount + committedEntriesCount);
            }

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                stateTable.GetShallowCopiesEnumerator(ActorStateType.LogicalTimestamp),
                sequenceNumber,
                sequenceNumber,
                baseCountPerType + 1);

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                stateTable.GetShallowCopiesEnumerator(ActorStateType.Reminder),
                sequenceNumber,
                sequenceNumber,
                baseCountPerType);

            TestCase("# Passed");
        }

        private void TestDeleteInternal(Dictionary<ActorStateType, int> statesPerReplication)
        {
            TestCase("##################");
            TestCase("### TestDelete ###");
            TestCase("##################");

            TestCase(
                "### StatesPerReplication (Actor:{0}, TimeStamp:{1}, Reminder:{2}) ###",
                statesPerReplication[ActorStateType.Actor],
                statesPerReplication[ActorStateType.LogicalTimestamp],
                statesPerReplication[ActorStateType.Reminder]);

            long sequenceNumber = 0;
            long committedEntriesCount = 0;
            long uncommittedEntriesCount = 0;

            var stateTable = new ActorStateTable();
            VerifyStateTableSnapshot(stateTable, statesPerReplication, long.MaxValue, 0, 0, 0);

            var actorReplicationUnitDict = new Dictionary<string, ReplicationUnit>();
            var timeStampReplicationUnitDict = new Dictionary<string, ReplicationUnit>();
            var reminderReplicationUnitDict = new Dictionary<string, ReplicationUnit>();

            TestCase("# Testcase 1: Single create/delete ...");
            {
                ++sequenceNumber;

                var keyPrefix = "x";

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    sequenceNumber);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                ++sequenceNumber;

                replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount -= statesPerReplication[ActorStateType.Actor] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                ++sequenceNumber;

                replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    1);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            TestCase("# Testcase 2: Multiple create/delete ...");

            foreach (var keyPrefix in new string[] { "a", "b", "c" })
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    1);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            var firstIteration = true;
            foreach (var keyPrefix in new string[] { "a", "b", "c" })
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];

                if (firstIteration)
                {
                    firstIteration = false;
                    committedEntriesCount -= statesPerReplication[ActorStateType.Actor] - 1;
                }
                else
                {
                    committedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                }

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            TestCase("# Testcase 3: Interleaved create/delete ...");

            foreach (var keyPrefix in new string[] { "d", "e", "f" })
            {
                ++sequenceNumber;

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    1);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                ++sequenceNumber;

                replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 2,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber - 1);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount -= statesPerReplication[ActorStateType.Actor] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            TestCase("# Testcase 4: Delete non-existent key ...");
            {
                ++sequenceNumber;

                var keyPrefix = "NotFound";

                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            {
                ++sequenceNumber;

                var keyPrefix = "Exists";

                var replicationUnit = ReplicationUnit.CreateForUpdateActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor],
                    1);

                actorReplicationUnitDict[keyPrefix] = replicationUnit;

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += statesPerReplication[ActorStateType.Actor] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            {
                ++sequenceNumber;

                var keyPrefix = "NotFound";

                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber - 1,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount += 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    0,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);

                VerifyReads(
                    stateTable,
                    actorReplicationUnitDict["Exists"],
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            TestCase("# Testcase 5: Delete same key different types ...");

            var timestamp = TimeSpan.FromSeconds(42);
            var reminderName = "Reminder-Exists";
            {
                ++sequenceNumber;

                var key = "Exists";

                var replicationUnit = ReplicationUnit.CreateForUpdateTimeStamp(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.LogicalTimestamp],
                    timestamp);

                timeStampReplicationUnitDict[key] = replicationUnit;

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    timestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];
                committedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp] - 1;

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    timestamp,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "Exists";

                var replicationUnit = ReplicationUnit.CreateForUpdateReminder(
                   sequenceNumber,
                   key,
                   statesPerReplication[ActorStateType.Reminder],
                   reminderName);

                reminderReplicationUnitDict[key] = replicationUnit;

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    false,
                    reminderName,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Reminder];
                committedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    reminderName,
                    sequenceNumber,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);
            }

            {
                ++sequenceNumber;

                var keyPrefix = "Exists";

                var replicationUnit = ReplicationUnit.CreateForDeleteTimeStamp(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.LogicalTimestamp]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.LogicalTimestamp];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    timestamp,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp];
                uncommittedEntriesCount -= statesPerReplication[ActorStateType.LogicalTimestamp] - 1;

                TryReadAndVerify(stateTable, timeStampReplicationUnitDict[keyPrefix], false, timestamp);
                TryReadAndVerify(stateTable, reminderReplicationUnitDict[keyPrefix], true, reminderName);
                TryReadAndVerify(stateTable, actorReplicationUnitDict[keyPrefix], true, 1);

                VerifyStateTableSnapshot(
                    stateTable,
                    statesPerReplication,
                    long.MaxValue,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            {
                ++sequenceNumber;

                var key = "Exists";

                var replicationUnit = ReplicationUnit.CreateForDeleteReminder(
                    sequenceNumber,
                    key,
                    statesPerReplication[ActorStateType.Reminder]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Reminder];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    reminderName,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Reminder];
                committedEntriesCount -= statesPerReplication[ActorStateType.Reminder];

                TryReadAndVerify(stateTable, timeStampReplicationUnitDict[key], false, timestamp);
                TryReadAndVerify(stateTable, reminderReplicationUnitDict[key], false, reminderName);
                TryReadAndVerify(stateTable, actorReplicationUnitDict[key], true, 1);

                VerifyStateTableSnapshot(
                    stateTable,
                    statesPerReplication,
                    long.MaxValue,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }

            {
                ++sequenceNumber;

                var keyPrefix = "Exists";

                var replicationUnit = ReplicationUnit.CreateForDeleteActor(
                    sequenceNumber,
                    keyPrefix,
                    statesPerReplication[ActorStateType.Actor]);

                TestPrepareUpdate(stateTable, replicationUnit);

                uncommittedEntriesCount += statesPerReplication[ActorStateType.Actor];

                VerifyReads(
                    stateTable,
                    replicationUnit,
                    statesPerReplication,
                    true,
                    1,
                    sequenceNumber - 1,
                    sequenceNumber,
                    uncommittedEntriesCount + committedEntriesCount);

                TestCommitUpdate(stateTable, sequenceNumber);

                uncommittedEntriesCount -= statesPerReplication[ActorStateType.Actor];
                committedEntriesCount -= statesPerReplication[ActorStateType.Actor];

                TryReadAndVerify(stateTable, timeStampReplicationUnitDict[keyPrefix], false, timestamp);
                TryReadAndVerify(stateTable, reminderReplicationUnitDict[keyPrefix], false, reminderName);
                TryReadAndVerify(stateTable, actorReplicationUnitDict[keyPrefix], false, 1);

                VerifyStateTableSnapshot(
                    stateTable,
                    statesPerReplication,
                    long.MaxValue,
                    sequenceNumber,
                    sequenceNumber,
                    committedEntriesCount + uncommittedEntriesCount);
            }
        }
    }
}
