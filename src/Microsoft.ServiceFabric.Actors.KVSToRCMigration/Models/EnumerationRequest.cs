// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// EnumerationRequest
    /// </summary>
    public class EnumerationRequest
    {
        /// <summary>
        /// Gets or Sets StartSN
        /// </summary>
        [Required]
        public long StartSN { get; set; }

        /// <summary>
        /// Gets or Sets NoOfItems
        /// </summary>
        [Required]
        public long NoOfItems { get; set; }

        /// <summary>
        /// Gets or Sets ChunkSize
        /// </summary>
        [Required]
        public long ChunkSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether includeDeletes
        /// </summary>
        public bool IncludeDeletes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether data intergrity checks are enabled.
        /// </summary>
        public bool ComputeHash { get; set; }
    }
}
