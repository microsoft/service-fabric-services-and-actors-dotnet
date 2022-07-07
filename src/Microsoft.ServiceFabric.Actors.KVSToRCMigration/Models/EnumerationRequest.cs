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
        /// Gets or Sets StartSN
        /// </summary>
        [DataMember]
        [Required]
        public long StartSN { get; set; }

        /// <summary>
        /// Gets or Sets NoOfItems
        /// </summary>
        [DataMember]
        [Required]
        public long NoOfItems { get; set; }

        /// <summary>
        /// Gets or Sets ChunkSize
        /// </summary>
        [DataMember]
        [Required]
        public long ChunkSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether includeDeletes
        /// </summary>
        [DataMember]
        [Required]
        public bool IncludeDeletes { get; set; }
    }
}
