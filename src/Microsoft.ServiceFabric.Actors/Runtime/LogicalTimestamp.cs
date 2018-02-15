// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    internal sealed class LogicalTimestamp
    {
        [DataMember]
        public TimeSpan Timestamp { get; private set; }

        public LogicalTimestamp(TimeSpan timestamp)
        {
            this.Timestamp = timestamp;
        }
    }
}
