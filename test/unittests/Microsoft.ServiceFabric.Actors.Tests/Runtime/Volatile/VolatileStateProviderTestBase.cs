// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime.Volatile
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using ActorStateData = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData;
    using ActorStateDataWrapper = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateTable<
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType,
        string,
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData>.ActorStateDataWrapper;
    using ActorStateTable = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateTable<
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType,
        string,
        Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateData>;
    using ActorStateType = Microsoft.ServiceFabric.Actors.Runtime.VolatileActorStateProvider.ActorStateType;

    /// <summary>
    /// Base class for VolatileStateProvider tests.
    /// </summary>
    public class VolatileStateProviderTestBase
    {
        private static object consoleLock = new object();

#pragma warning disable SA1600 // Elements should be documented. skip for test methods

        /// <summary>
        /// Logs a string to console.
        /// </summary>
        /// <param name="format">string foramt to log.</param>
        /// <param name="args">arguments for string foramt.</param>
        public static void TestCase(string format, params object[] args)
        {
            TestLog("\n");
            TestLog(ConsoleColor.Green, format, args);
            TestLog("\n");
        }

        /// <summary>
        /// Logs a string to console.
        /// </summary>
        /// <param name="format">string foramt to log.</param>
        /// <param name="args">arguments for string foramt.</param>
        public static void TestLog(string format, params object[] args)
        {
            TestLog(ConsoleColor.Gray, format, args);
        }

        /// <summary>
        /// Logs a string to console.
        /// </summary>
        /// <param name="color">Color to use for logging to console.</param>
        /// <param name="format">string foramt to log.</param>
        /// <param name="args">arguments for string foramt.</param>
        public static void TestLog(ConsoleColor color, string format, params object[] args)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(format, args);
                Console.ResetColor();
            }
        }

        internal static void TestPrepareUpdate(ActorStateTable stateTable, ReplicationUnit replicationUnit)
        {
            foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
            {
                TestLog(
                    "PrepareUpdate(SequenceNumber: {0}, Type: {1} Key: {2}, IsDelete: {3})",
                    replicationUnit.SequenceNumber,
                    actorStateDataWrapper.Type.ToString(),
                    actorStateDataWrapper.Key,
                    actorStateDataWrapper.IsDelete);
            }

            stateTable.PrepareUpdate(replicationUnit.ActorStateDataWrapperList, replicationUnit.SequenceNumber);
        }

        internal static void TestCommitUpdate(ActorStateTable stateTable, long sequenceNumber)
        {
            TestLog("Commiting Sequence Number ({0})...", sequenceNumber);
            stateTable.CommitUpdateAsync(sequenceNumber).Wait();
            TestLog("Commited Sequence Number ({0})", sequenceNumber);
        }

        internal static void TestApply(ActorStateTable stateTable, ReplicationUnit replicationUnit, bool testLog = true)
        {
            if (testLog)
            {
                foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
                {
                    TestLog(
                        "Apply(SequenceNumber: {0}, Key: {1}, IsDelete: {2})",
                        replicationUnit.SequenceNumber,
                        actorStateDataWrapper.Key,
                        actorStateDataWrapper.IsDelete);
                }
            }

            replicationUnit.UpdateSequenceNumber();

            stateTable.ApplyUpdates(replicationUnit.ActorStateDataWrapperList);
        }

        /// <summary>
        /// The replicationUnitList should enumerate the replicationUnit in increasing order of sequence number.
        /// </summary>
        /// <param name="stateTable">Actor state table.</param>
        /// <param name="replicationUnitList">List of ReplicationUnit.</param>
        /// <returns>List of list of ActorStateDataWrapper.</returns>
        internal static List<List<ActorStateDataWrapper>> TestApplyBatch(
            ActorStateTable stateTable,
            IEnumerable<ReplicationUnit> replicationUnitList)
        {
            var replicationData = new List<List<ActorStateDataWrapper>>();
            var batch = new List<ActorStateDataWrapper>();

            foreach (var replicationUnit in replicationUnitList)
            {
                replicationUnit.UpdateSequenceNumber();

                replicationData.Add(replicationUnit.ActorStateDataWrapperList);
                batch.AddRange(replicationUnit.ActorStateDataWrapperList);

                TestLog(
                    "ApplyBatch({0}, {1}, {2})",
                    replicationUnit.SequenceNumber,
                    replicationUnit.StateType,
                    replicationUnit.GetKeyString());
            }

            stateTable.ApplyUpdates(batch);

            return replicationData;
        }

        internal static void VerifyReads(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            Dictionary<ActorStateType, int> statesPerReplication,
            bool expectedResult,
            long expectedLength,
            long expectedCommittedSequenceNumber,
            long expectedKnownSequenceNumber,
            long expectedCount)
        {
            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                long.MaxValue,
                expectedCommittedSequenceNumber,
                expectedKnownSequenceNumber,
                expectedCount);

            TryReadAndVerify(stateTable, replicationUnit, expectedResult, expectedLength);
        }

        internal static void VerifyReads(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            Dictionary<ActorStateType, int> statesPerReplication,
            bool expectedResult,
            TimeSpan timeStamp,
            long expectedCommittedSequenceNumber,
            long expectedKnownSequenceNumber,
            long expectedCount)
        {
            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                long.MaxValue,
                expectedCommittedSequenceNumber,
                expectedKnownSequenceNumber,
                expectedCount);

            TryReadAndVerify(stateTable, replicationUnit, expectedResult, timeStamp);
        }

        internal static void VerifyReads(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            Dictionary<ActorStateType, int> statesPerReplication,
            bool expectedResult,
            string reminderName,
            long expectedCommittedSequenceNumber,
            long expectedKnownSequenceNumber,
            long expectedCount)
        {
            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                long.MaxValue,
                expectedCommittedSequenceNumber,
                expectedKnownSequenceNumber,
                expectedCount);

            TryReadAndVerify(stateTable, replicationUnit, expectedResult, reminderName);
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            bool expectedResult,
            long expectedLength)
        {
            foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
            {
                TryReadAndVerify(
                    stateTable,
                    actorStateDataWrapper.Key,
                    expectedResult,
                    expectedLength);
            }
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            string key,
            bool expectedResult,
            long expectedLength)
        {
            var actualResult = TryGetActorStateData(stateTable, ActorStateType.Actor, key, expectedResult, out var data);

            if (actualResult)
            {
                var resultBuffer = data.ActorState;
                FailTestIf(
                    expectedLength != resultBuffer.Length,
                    "DataLength({0}): expected={1} actual={2}",
                    key,
                    expectedLength,
                    resultBuffer.Length);
            }
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            bool expectedResult,
            TimeSpan expectedTimestamp)
        {
            foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
            {
                TryReadAndVerify(
                    stateTable,
                    actorStateDataWrapper.Key,
                    expectedResult,
                    expectedTimestamp);
            }
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            string key,
            bool expectedResult,
            TimeSpan expectedTimestamp)
        {
            var actualResult = TryGetActorStateData(stateTable, ActorStateType.LogicalTimestamp, key, expectedResult, out var data);

            if (actualResult)
            {
                var resultTimestamp = data.LogicalTimestamp.Value;
                FailTestIf(
                    expectedTimestamp != resultTimestamp,
                    "Data({0}): expected={1} actual={2}",
                    key,
                    expectedTimestamp,
                    resultTimestamp);
            }
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            ReplicationUnit replicationUnit,
            bool expectedResult,
            string expectedReminderName)
        {
            foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
            {
                TryReadAndVerify(
                    stateTable,
                    actorStateDataWrapper.Key,
                    expectedResult,
                    expectedReminderName);
            }
        }

        internal static void TryReadAndVerify(
            ActorStateTable stateTable,
            string key,
            bool expectedResult,
            string expectedReminderName)
        {
            var actualResult = TryGetActorStateData(stateTable, ActorStateType.Reminder, key, expectedResult, out var data);

            if (actualResult)
            {
                var resultReminder = data.ActorReminderData;
                FailTestIf(
                    expectedReminderName != resultReminder.Name,
                    "Data({0}): expected={1} actual={2}",
                    key,
                    expectedReminderName,
                    resultReminder.Name);
            }
        }

        internal static void VerifyStateTableSnapshot(
            ActorStateTable stateTable,
            Dictionary<ActorStateType, int> statesPerReplication,
            long maxSequenceNumber,
            long expectedCommittedSequenceNumber,
            long expectedKnownSequenceNumber,
            long expectedCount)
        {
            var enumerator = stateTable.GetShallowCopiesEnumerator(maxSequenceNumber);

            VerifyStateTableSnapshot(
                stateTable,
                statesPerReplication,
                enumerator,
                expectedCommittedSequenceNumber,
                expectedKnownSequenceNumber,
                expectedCount);
        }

        /// <summary>
        /// This function verifies snapshot when the replication units are uniform
        /// i.e. they either contain update entries or delete entries.
        /// </summary>
        /// <param name="stateTable">Actor state table.</param>
        /// <param name="statesPerReplication">States per replictaion.</param>
        /// <param name="enumerator">Actor state enumerator.</param>
        /// <param name="expectedCommittedSequenceNumber">expected CommittedSequenceNumber</param>
        /// <param name="expectedKnownSequenceNumber">expected KnownSequenceNumber</param>
        /// <param name="expectedCount">expected Count</param>
        internal static void VerifyStateTableSnapshot(
            ActorStateTable stateTable,
            Dictionary<ActorStateType, int> statesPerReplication,
            ActorStateTable.ActorStateEnumerator enumerator,
            long expectedCommittedSequenceNumber,
            long expectedKnownSequenceNumber,
            long expectedCount)
        {
            var actualCommittedSequenceNumber = stateTable.GetHighestCommittedSequenceNumber();
            var actualKnownSequenceNumber = stateTable.GetHighestKnownSequenceNumber();

            TestLog(
                "ActorStateTable committed:{0} known:{1}",
                stateTable.GetHighestCommittedSequenceNumber(),
                stateTable.GetHighestKnownSequenceNumber());

            FailTestIf(
                expectedCommittedSequenceNumber != actualCommittedSequenceNumber,
                "Committed sequence number: expected:{0} actual:{1}",
                expectedCommittedSequenceNumber,
                actualCommittedSequenceNumber);

            FailTestIf(
                expectedKnownSequenceNumber != actualKnownSequenceNumber,
                "Known sequence number: expected:{0} actual:{1}",
                expectedKnownSequenceNumber,
                actualKnownSequenceNumber);

            // Group the actor state by replication unit
            var replicationUnits = GroupByRelicationUnit(enumerator);

            long actualCount = 0;
            long lastSequenceNumber = 0;

            foreach (var replicationUnit in replicationUnits)
            {
                TestLog(
                "[{0}] '{1}' {2}",
                replicationUnit.SequenceNumber,
                replicationUnit.StateType,
                replicationUnit.GetKeyString());

                FailTestIf(
                replicationUnit.SequenceNumber <= lastSequenceNumber,
                "Sequence number: previous:{0} current:{1}",
                lastSequenceNumber,
                replicationUnit.SequenceNumber);

                if ((replicationUnit.SequenceNumber == actualCommittedSequenceNumber) &&
                    replicationUnit.IsLastActorStateDelete)
                {
                    FailTestIf(
                            replicationUnit.ActorStateCount != 1,
                            "States in replication unit at end of committed entry list: IsDelete:{0} ActualStateCount:{1} ExpectedStateCount:1",
                            replicationUnit.IsLastActorStateDelete,
                            replicationUnit.ActorStateCount);
                }
                else if (replicationUnit.SequenceNumber <= actualCommittedSequenceNumber)
                {
                    // The replication unit should only contain update entries
                    FailTestIf(
                        replicationUnit.ActorStateCount != statesPerReplication[replicationUnit.StateType],
                        "States in replication unit: ActualCount:{0} ExpectedCount:{1}",
                        replicationUnit.ActorStateCount,
                        statesPerReplication[replicationUnit.StateType]);

                    FailTestIf(
                        replicationUnit.ActorStateUpdateCount != statesPerReplication[replicationUnit.StateType],
                        "Update states in replication unit: ActualUpdateCount:{0} ExpectedUpdateCount:{1}",
                        replicationUnit.ActorStateUpdateCount,
                        statesPerReplication[replicationUnit.StateType]);
                }
                else
                {
                    // The replication unit should only contain either update entries or  delete entries
                    if (replicationUnit.IsLastActorStateDelete)
                    {
                        FailTestIf(
                            replicationUnit.ActorStateUpdateCount != 0,
                            "States in replication unit: IsDelete:{0} ActualUpdateStateCount:{1} ExpectedUpdateStateCount:0",
                            replicationUnit.IsLastActorStateDelete,
                            replicationUnit.ActorStateUpdateCount);

                        FailTestIf(
                            replicationUnit.ActorStateCount != statesPerReplication[replicationUnit.StateType],
                            "States in replication unit: IsDelete:{0} ActualStateCount:{1} ExpectedStateCount:{2}",
                            replicationUnit.IsLastActorStateDelete,
                            replicationUnit.ActorStateCount,
                            statesPerReplication[replicationUnit.StateType]);
                    }
                    else
                    {
                        FailTestIf(
                            replicationUnit.ActorStateUpdateCount != statesPerReplication[replicationUnit.StateType],
                            "States in replication unit: IsDelete:{0} ActualUpdateStateCount:{1} ExpectedUpdateStateCount:{2}",
                            replicationUnit.IsLastActorStateDelete,
                            replicationUnit.ActorStateUpdateCount,
                            statesPerReplication[replicationUnit.StateType]);

                        FailTestIf(
                            replicationUnit.ActorStateCount != statesPerReplication[replicationUnit.StateType],
                            "States in replication unit: IsDelete:{0} ActualStateCount:{1} ExpectedStateCount:{2}",
                            replicationUnit.IsLastActorStateDelete,
                            replicationUnit.ActorStateCount,
                            statesPerReplication[replicationUnit.StateType]);
                    }
                }

                foreach (var actorStateDataWrapper in replicationUnit.ActorStateDataWrapperList)
                {
                    var dataString = GetDataString(actorStateDataWrapper);

                    TestLog(
                        "[{0}] '{1}' {2} {3}",
                        actorStateDataWrapper.SequenceNumber,
                        actorStateDataWrapper.Type,
                        actorStateDataWrapper.Key,
                        dataString);

                    FailTestIf(
                        actorStateDataWrapper.SequenceNumber != replicationUnit.SequenceNumber,
                        "Sequence number same as replication unit: RU ={0} ActorState={1}",
                        replicationUnit.SequenceNumber,
                        actorStateDataWrapper.SequenceNumber);

                    FailTestIf(
                        actorStateDataWrapper.SequenceNumber != replicationUnit.SequenceNumber,
                        "State type same as replication unit: RU ={0} ActorState={1}",
                        replicationUnit.StateType,
                        actorStateDataWrapper.Type);

                    FailTestIf(string.IsNullOrEmpty(dataString), "Data not null or empty");
                }

                lastSequenceNumber = replicationUnit.SequenceNumber;
                actualCount += replicationUnit.ActorStateDataWrapperList.Count;
            }

            FailTestIf(
                expectedCount != actualCount,
                "Total: expected:{0} actual:{1}\n",
                expectedCount,
                actualCount);
        }

        internal static string GetDataString(ActorStateDataWrapper actorStateDataWrapper)
        {
            string dataString = null;

            if (actorStateDataWrapper.IsDelete)
            {
                dataString = "Delete";
            }
            else if (actorStateDataWrapper.Value.ActorReminderData != null)
            {
                dataString = actorStateDataWrapper.Value.ActorReminderData.Name;
            }
            else if (actorStateDataWrapper.Value.LogicalTimestamp.HasValue)
            {
                dataString = actorStateDataWrapper.Value.LogicalTimestamp.Value.ToString();
            }
            else if (actorStateDataWrapper.Value.ActorState != null)
            {
                dataString = string.Format("{0} bytes", actorStateDataWrapper.Value.ActorState.Length);
            }

            return dataString;
        }

        internal static void ValidateStateData(ActorStateDataWrapper stateData, string dataString, long lastSequenceNumber)
        {
            TestLog(
                "[{0}] '{1}' {2} {3}",
                stateData.SequenceNumber,
                stateData.Type,
                stateData.Key,
                dataString);

            FailTestIf(string.IsNullOrEmpty(dataString), "Data not null or empty");

            FailTestIf(
                stateData.SequenceNumber <= lastSequenceNumber,
                "Sequence number: previous={0} current={1}",
                lastSequenceNumber,
                stateData.SequenceNumber);
        }

        internal static ActorStateDataWrapper CreateActorStateDataWrapper(string key, long dataLength)
        {
            var bytes = new byte[dataLength];
            for (var ix = 0; ix < dataLength; ++ix)
            {
                bytes[ix] = (byte)ix;
            }

            return ActorStateDataWrapper.CreateForUpdate(ActorStateType.Actor, key, new ActorStateData(bytes));
        }

        internal static ActorStateDataWrapper CreateActorStateDataWrapper(string key, TimeSpan timestamp)
        {
            return ActorStateDataWrapper.CreateForUpdate(ActorStateType.LogicalTimestamp, key, new ActorStateData(timestamp));
        }

        internal static ActorStateDataWrapper CreateActorStateDataWrapper(string key, string reminderName)
        {
            var reminder = new ActorReminderData(
                new ActorId(reminderName),
                new MockReminder(reminderName),
                TimeSpan.FromSeconds(reminderName.Length));

            return ActorStateDataWrapper.CreateForUpdate(ActorStateType.Reminder, key, new ActorStateData(reminder));
        }

        internal static ActorStateDataWrapper CreateActorStateDataWrapperForDelete(ActorStateType type, string key)
        {
            return ActorStateDataWrapper.CreateForDelete(type, key);
        }

        internal static List<ReplicationUnit> GroupByRelicationUnit(ActorStateTable.ActorStateEnumerator enumerator)
        {
            var replicationUnits = new List<ReplicationUnit>();

            while (enumerator.PeekNext() != null)
            {
                var peek = enumerator.PeekNext();
                var replicationUnit = new ReplicationUnit(peek.SequenceNumber, peek.Type);

                do
                {
                    replicationUnit.ActorStateDataWrapperList.Add(enumerator.GetNext());
                    peek = enumerator.PeekNext();
                }
                while (peek != null && peek.SequenceNumber == replicationUnit.SequenceNumber);

                replicationUnits.Add(replicationUnit);
            }

            return replicationUnits;
        }

        internal static Dictionary<ActorStateType, int> GetStatesPerReplication(
            int actorstatesPerReplication = 1,
            int timeStampstatesPerReplication = 1,
            int reminderStatesPerReplication = 1)
        {
            var statesPerReplication = new Dictionary<ActorStateType, int>
            {
                [ActorStateType.Actor] = actorstatesPerReplication,
                [ActorStateType.LogicalTimestamp] = timeStampstatesPerReplication,
                [ActorStateType.Reminder] = reminderStatesPerReplication,
            };

            return statesPerReplication;
        }

        /// <summary>
        /// Fails the test by throwinf exception.
        /// </summary>
        /// <param name="condition">Condition to check for test pass or failure.</param>
        /// <param name="format">string foramt to log.</param>
        /// <param name="args">arguments for string foramt.</param>
        protected static void FailTestIf(bool condition, string format, params object[] args)
        {
            var conditionMessage = string.Format(CultureInfo.InvariantCulture, format, args);

            if (condition)
            {
                var failedFormat = "Failed condition: {0}";

                TestLog(ConsoleColor.Red, failedFormat, conditionMessage);

                throw new InvalidOperationException(string.Format(
                    CultureInfo.InvariantCulture,
                    failedFormat,
                    conditionMessage));
            }
            else
            {
                TestLog(ConsoleColor.DarkGray, "Passed condition: {0}", conditionMessage);
            }
        }

        private static bool TryGetActorStateData(
            ActorStateTable stateTable,
            ActorStateType type,
            string key,
            bool expectedResult,
            out ActorStateData data)
        {
            data = null;
            var actualResult = stateTable.TryGetValue(type, key, out data);

            FailTestIf(
                expectedResult != actualResult,
                "TryGetValue({0}, {1}): expected={2} actual={3}",
                type,
                key,
                expectedResult,
                actualResult);

            return actualResult;
        }

        internal class MockReminder : IActorReminder
        {
            private readonly string name;

            public MockReminder(string reminderName)
            {
                this.name = reminderName;
            }

            TimeSpan IActorReminder.DueTime
            {
                get { return TimeSpan.MaxValue; }
            }

            string IActorReminder.Name
            {
                get { return this.name; }
            }

            TimeSpan IActorReminder.Period
            {
                get { return TimeSpan.MaxValue; }
            }

            byte[] IActorReminder.State
            {
                get { return null; }
            }
        }

        internal class ReplicationUnit
        {
            internal ReplicationUnit(long sequenceNumber, ActorStateType stateType)
            {
                this.SequenceNumber = sequenceNumber;
                this.StateType = stateType;
                this.ActorStateDataWrapperList = new List<ActorStateDataWrapper>();
            }

            internal long SequenceNumber { get; private set; }

            internal List<ActorStateDataWrapper> ActorStateDataWrapperList { get; private set; }

            internal ActorStateType StateType { get; private set; }

            internal int ActorStateCount
            {
                get
                {
                    return this.ActorStateDataWrapperList.Count;
                }
            }

            internal int ActorStateUpdateCount
            {
                get
                {
                    var count = 0;

                    foreach (var actorStateDataWrapper in this.ActorStateDataWrapperList)
                    {
                        if (!actorStateDataWrapper.IsDelete)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }

            internal bool IsLastActorStateDelete
            {
                get
                {
                    return this.ActorStateDataWrapperList[this.ActorStateCount - 1].IsDelete;
                }
            }

            internal static ReplicationUnit CreateForUpdateActor(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication,
                long dataLength)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.Actor);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(CreateActorStateDataWrapper(keyPrefix + i, dataLength));
                }

                return replicationUnit;
            }

            internal static ReplicationUnit CreateForDeleteActor(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.Actor);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(
                        CreateActorStateDataWrapperForDelete(ActorStateType.Actor, keyPrefix + i));
                }

                return replicationUnit;
            }

            internal static ReplicationUnit CreateForUpdateTimeStamp(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication,
                TimeSpan timeStamp)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.LogicalTimestamp);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(CreateActorStateDataWrapper(keyPrefix + i, timeStamp));
                }

                return replicationUnit;
            }

            internal static ReplicationUnit CreateForDeleteTimeStamp(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.LogicalTimestamp);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(
                        CreateActorStateDataWrapperForDelete(ActorStateType.LogicalTimestamp, keyPrefix + i));
                }

                return replicationUnit;
            }

            internal static ReplicationUnit CreateForUpdateReminder(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication,
                string reminderName)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.Reminder);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(CreateActorStateDataWrapper(keyPrefix + i, reminderName));
                }

                return replicationUnit;
            }

            internal static ReplicationUnit CreateForDeleteReminder(
                long sequenceNumber,
                string keyPrefix,
                int statesPerReplication)
            {
                var replicationUnit = new ReplicationUnit(sequenceNumber, ActorStateType.Reminder);

                for (var i = 1; i <= statesPerReplication; i++)
                {
                    replicationUnit.ActorStateDataWrapperList.Add(
                        CreateActorStateDataWrapperForDelete(ActorStateType.Reminder, keyPrefix + i));
                }

                return replicationUnit;
            }

            internal void UpdateSequenceNumber()
            {
                foreach (var actorStateDataWrapper in this.ActorStateDataWrapperList)
                {
                    actorStateDataWrapper.UpdateSequenceNumber(this.SequenceNumber);
                }
            }

            internal string GetKeyString()
            {
                var keysString = "[ ";

                foreach (var actorStateDataWrapper in this.ActorStateDataWrapperList)
                {
                    keysString += actorStateDataWrapper.Key + " ";
                }

                keysString += "]";

                return keysString;
            }
        }
#pragma warning restore SA1600 // Elements should be documented
    }
}
