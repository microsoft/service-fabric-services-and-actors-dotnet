// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// KeyValuePair
    /// </summary>
    [DataContract]
    public class MigrationStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationStatus"/> class.
        /// </summary>
        public MigrationStatus()
        {
            this.WorkerStatuses = new List<WorkerStatus>();
        }

        /// <summary>
        /// Gets or Sets ParitionId
        /// </summary>
        [DataMember]
        public Guid ParitionId { get; set; }

        /// <summary>
        /// Gets or Sets MigrationStartTimeUtc
        /// </summary>
        [DataMember]
        public DateTime MigrationStartTimeUtc { get; set; }

        /// <summary>
        /// Gets or Sets CurrentMigrationPhaseStartTimeUtc
        /// </summary>
        [DataMember]
        public DateTime CurrentMigrationPhaseStartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets CurrentMigrationPhase
        /// </summary>
        [DataMember]
        public string CurrentMigrationPhase { get; set; }

        /// <summary>
        /// Gets or Sets KVS_LSN
        /// </summary>
        [DataMember]
        public long KVS_LSN { get; set; }

        /// <summary>
        /// Gets or Sets WorkerStatuses
        /// </summary>
        [DataMember]
        public List<WorkerStatus> WorkerStatuses { get; set; }
    }
}
