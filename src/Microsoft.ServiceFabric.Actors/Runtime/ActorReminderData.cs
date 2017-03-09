// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{                                      
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    internal sealed class ActorReminderData
    {
        [DataMember]
        internal ActorId ActorId { get; private set; }

        [DataMember]
        internal string Name { get; private set; }

        [DataMember]
        internal TimeSpan DueTime { get; private set; }

        [DataMember]
        internal TimeSpan Period { get; private set; }

        [DataMember]
        internal byte[] State { get; private set; }

        [DataMember]
        internal TimeSpan LogicalCreationTime { get; private set; }

        [DataMember]
        internal bool IsReadOnly { get; private set; } // Redundant. Not used anymore.

        public ActorReminderData(ActorId actorId, string name, TimeSpan dueTime, TimeSpan period, byte[] state, TimeSpan logicalCreationTime)
        {
            this.ActorId = actorId;
            this.Name = name;
            this.DueTime = dueTime;
            this.Period = period;
            this.State = state;
            this.LogicalCreationTime = logicalCreationTime;
        }

        public ActorReminderData(ActorId actorId, IActorReminder reminder, TimeSpan logicalCreationTime)
        {
            this.ActorId = actorId;
            this.Name = reminder.Name;
            this.DueTime = reminder.DueTime;
            this.Period = reminder.Period;
            this.State = reminder.State;
            this.LogicalCreationTime = logicalCreationTime;
        }

        public long EstimateDataLength()
        {
            return this.ActorId.EstimateDataLength()
                   + (this.Name.Length*sizeof(char))
                   + sizeof(long) // DueTime
                   + sizeof(long) // Period
                   + this.State.Length*sizeof(byte)
                   + sizeof(long) // Attributes
                   + sizeof(long); // LogicalCreationTime
        }
    }
}