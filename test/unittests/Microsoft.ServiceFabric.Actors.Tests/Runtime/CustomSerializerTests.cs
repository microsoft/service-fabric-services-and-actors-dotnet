// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
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
            ActorReminderDataSerializer.Deserialize(ActorReminderDataSerializer.Serialize(null))
                .Should()
                .BeNull("ActorReminderData is null");

            foreach (var data in GetActorReminderList())
            {
                var deserializedData =
                    ActorReminderDataSerializer.Deserialize(ActorReminderDataSerializer.Serialize(data));

                deserializedData.ActorId.Should().Be(data.ActorId);
                deserializedData.Name.Should().Be(data.Name);
                deserializedData.DueTime.Should().Be(data.DueTime);
                deserializedData.Period.Should().Be(data.Period);

                if (data.State == null)
                {
                    deserializedData.State.Should().BeNull("ActorReminder.State serialization");
                }
                else
                {
                    data.State.SequenceEqual(deserializedData.State)
                        .Should()
                        .BeTrue("ActorReminder.State serialization");
                }

                deserializedData.LogicalCreationTime.Should().Be(data.LogicalCreationTime, "ActorReminder.LogicalCreationTime serialization.");
            }
        }

        /// <summary>
        /// Tests ReminderCompletedDataSerialization.
        /// </summary>
        [Fact]
        public void VerifyReminderCompletedDataSerialization()
        {
            ReminderCompletedDataSerializer.Deserialize(ReminderCompletedDataSerializer.Serialize(null))
                .Should()
                .BeNull("Null ReminderCompletedDataSerializer serialization");

            var data = new ReminderCompletedData(TimeSpan.MinValue, DateTime.MaxValue);
            var deserializedData = ReminderCompletedDataSerializer.Deserialize(ReminderCompletedDataSerializer.Serialize(data));

            deserializedData.LogicalTime.Should().Be(data.LogicalTime, "ReminderCompletedData.LogicalTime.");
            deserializedData.UtcTime.Should().Be(data.UtcTime, "ReminderCompletedData.UtcTime.");
        }

        /// <summary>
        /// Tests LogicalTimestampSerialization.
        /// </summary>
        [Fact]
        public void VerifyLogicalTimestampSerialization()
        {
            LogicalTimestampSerializer.Deserialize(
                LogicalTimestampSerializer.Serialize(null))
                    .Should()
                    .BeNull("Null LogicalTimestampSerializer serialization");

            var data = new LogicalTimestamp(TimeSpan.MaxValue);
            var deserializedData = LogicalTimestampSerializer.Deserialize(LogicalTimestampSerializer.Serialize(data));

            deserializedData.Timestamp.Should().Be(data.Timestamp, "LogicalTimestamp.Timestamp serialization.");
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
