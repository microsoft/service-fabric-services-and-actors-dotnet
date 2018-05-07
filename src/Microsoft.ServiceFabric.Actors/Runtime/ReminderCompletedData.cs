// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    internal sealed class ReminderCompletedData
    {
        [DataMember]
        private readonly TimeSpan logicalTime;

        [DataMember]
        private readonly DateTime utcTime;

        public ReminderCompletedData(TimeSpan logicalTime, DateTime utcTime)
        {
            this.logicalTime = logicalTime;
            this.utcTime = utcTime;
        }

        public TimeSpan LogicalTime
        {
            get { return this.logicalTime; }
        }

        public DateTime UtcTime
        {
            get { return this.utcTime; }
        }

        public long EstimateDataLength()
        {
            return 2 * sizeof(long); // LogicalTime + UtcTime
        }
    }
}
