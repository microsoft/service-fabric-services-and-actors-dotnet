// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// EnumerationRequest
    /// </summary>
    [DataContract]
    public class EnumerationRequest
    {
        /// <summary>
        /// Gets or Sets start sequence number per enumeration.
        /// </summary>
        [DataMember]
        [Required]
        public long StartSequenceNumber { get; set; }

        /// <summary>
        /// Gets or Sets end sequence number per enumeration.
        /// </summary>
        [DataMember]
        [Required]
        public long EndSequenceNumber { get; set; }

        /// <summary>
        /// Gets or Sets ChunkSize
        /// </summary>
        [DataMember]
        [Required]
        public long ChunkSize { get; set; }

        /// <summary>
        /// Gets or Sets number of chunks per enumeration.
        /// </summary>
        [DataMember]
        [Required]
        public int NumberOfChunksPerEnumeration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include tombstones in the response.
        /// </summary>
        [DataMember]
        [Required]
        public bool IncludeDeletes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include actorids for actor state KV pairs.
        /// </summary>
        [DataMember]
        public bool ResolveActorIdsForStateKVPairs { get; set; }
    }
}
