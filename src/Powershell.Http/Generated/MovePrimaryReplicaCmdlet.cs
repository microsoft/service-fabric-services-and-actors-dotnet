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
    /// Moves the primary replica of a partition of a stateful service.
    /// </summary>
    [Cmdlet(VerbsCommon.Move, "SFPrimaryReplica")]
    public partial class MovePrimaryReplicaCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets IgnoreConstraints. Ignore constraints when moving a replica or instance. If this parameter is not
        /// specified, all constraints are honored.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public bool? IgnoreConstraints { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            this.ServiceFabricClient.Partitions.MovePrimaryReplicaAsync(
                partitionId: this.PartitionId,
                nodeName: this.NodeName,
                ignoreConstraints: this.IgnoreConstraints,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
