// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// KeyValuePair
    /// </summary>
    [DataContract]
    public class WorkerStatus
    {
        /// <summary>
        /// Gets or Sets WorkerId
        /// </summary>
        [DataMember]
        public string WorkerId { get; set; }

        /// <summary>
        /// Gets or Sets FirstAppliedSeqNum
        /// </summary>
        [DataMember]
        public long FirstAppliedSeqNum { get; set; }

        /// <summary>
        /// Gets or Sets LastAppliedSeqNum
        /// </summary>
        [DataMember]
        public long LastAppliedSeqNum { get; set; }
    }
}
