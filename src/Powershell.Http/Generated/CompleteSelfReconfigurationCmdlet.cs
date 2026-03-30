// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Forces completion of a stuck self-reconfiguration on a partition.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Complete, "SFSelfReconfiguration")]
    public partial class CompleteSelfReconfigurationCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The partition ID of the service with the stuck reconfiguration.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public Guid? PartitionId { get; set; }

        /// <summary>
        /// Gets or sets RequestSequenceNumber. The sequence number from the reconfiguration request. This value must match
        /// ReportId.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public long? RequestSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets RequestGenerationNumber. The generation number from the reconfiguration request. Identifies which
        /// generation of the request to complete.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public long? RequestGenerationNumber { get; set; }

        /// <summary>
        /// Gets or sets ReportId. The reconfiguration report sequence number. This value must match RequestSequenceNumber.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3)]
        public long? ReportId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var completeSelfReconfigurationDescription = new CompleteSelfReconfigurationDescription(
            partitionId: this.PartitionId,
            requestSequenceNumber: this.RequestSequenceNumber,
            requestGenerationNumber: this.RequestGenerationNumber,
            reportId: this.ReportId);

            this.ServiceFabricClient.Partitions.CompleteSelfReconfigurationAsync(
                completeSelfReconfigurationDescription: completeSelfReconfigurationDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
