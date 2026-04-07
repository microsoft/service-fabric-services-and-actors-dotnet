// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Xunit;

    /// <summary>
    /// Class containing tests for ActorReminderDataSerializer.
    /// </summary>
    public class CustomSerializerTests
    {
        /// <summary>
        /// Tests ActorReminderDataSerialization.
        /// </summary>
        [Fact]
        public void VerifyActorReminderDataSerialization()
        {
            Assert.Null(ActorReminderDataSerializer.Deserialize(ActorReminderDataSerializer.Serialize(null)));

            foreach (var data in GetActorReminderList())
            {
                var deserializedData =
                    ActorReminderDataSerializer.Deserialize(ActorReminderDataSerializer.Serialize(data));

                Assert.Equal(data.ActorId, deserializedData.ActorId);
                Assert.Equal(data.Name, deserializedData.Name);
                Assert.Equal(data.DueTime, deserializedData.DueTime);
                Assert.Equal(data.Period, deserializedData.Period);

                if (data.State == null)
                {
                    Assert.Null(deserializedData.State);
                }
                else
                {
                    Assert.True(data.State.SequenceEqual(deserializedData.State));
                }

                Assert.Equal(data.LogicalCreationTime, deserializedData.LogicalCreationTime);
            }
        }

        /// <summary>
        /// Tests ReminderCompletedDataSerialization.
        /// </summary>
        [Fact]
        public void VerifyReminderCompletedDataSerialization()
        {
            Assert.Null(ReminderCompletedDataSerializer.Deserialize(ReminderCompletedDataSerializer.Serialize(null)));

            var data = new ReminderCompletedData(TimeSpan.MinValue, DateTime.MaxValue);
            var deserializedData = ReminderCompletedDataSerializer.Deserialize(ReminderCompletedDataSerializer.Serialize(data));

            Assert.Equal(data.LogicalTime, deserializedData.LogicalTime);
            Assert.Equal(data.UtcTime, deserializedData.UtcTime);
        }

        /// <summary>
        /// Tests LogicalTimestampSerialization.
        /// </summary>
        [Fact]
        public void VerifyLogicalTimestampSerialization()
        {
            Assert.Null(LogicalTimestampSerializer.Deserialize(LogicalTimestampSerializer.Serialize(null)));

            var data = new LogicalTimestamp(TimeSpan.MaxValue);
            var deserializedData = LogicalTimestampSerializer.Deserialize(LogicalTimestampSerializer.Serialize(data));

            Assert.Equal(data.Timestamp, deserializedData.Timestamp);
        }

        private static List<ActorReminderData> GetActorReminderList()
        {
            var actorIds = new List<ActorId> { null, new ActorId(Guid.NewGuid()), ActorId.CreateRandom(), new ActorId(Guid.NewGuid().ToString()) };
            var reminderNames = new List<string> { null, string.Empty, Guid.NewGuid().ToString() };
            var reminderStates = new List<byte[]> { null, new byte[0], new byte[16] };

            var actorReminderDataList = new List<ActorReminderData>();

            foreach (var actorId in actorIds)
            {
                foreach (var reminderName in reminderNames)
                {
                    foreach (var reminderState in reminderStates)
                    {
                        actorReminderDataList.Add(
                            new ActorReminderData(actorId, reminderName, TimeSpan.MaxValue, TimeSpan.MinValue, reminderState, TimeSpan.Zero));
                    }
                }
            }

            return actorReminderDataList;
        }
    }
}
