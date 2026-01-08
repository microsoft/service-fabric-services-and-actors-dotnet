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
    /// Moves the auxiliary replica of a partition of a stateful service.
    /// </summary>
    [Cmdlet(VerbsCommon.Move, "SFAuxiliaryReplica")]
    public partial class MoveAuxiliaryReplicaCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets CurrentNodeName. The name of the source node for instance move. If not specified, instance is moved
        /// from a random node.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public NodeName CurrentNodeName { get; set; }

        /// <summary>
        /// Gets or sets NewNodeName. The name of the target node for secondary replica or instance move. If not specified,
        /// replica or instance is moved to a random node.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public NodeName NewNodeName { get; set; }

        /// <summary>
        /// Gets or sets IgnoreConstraints. Ignore constraints when moving a replica or instance. If this parameter is not
        /// specified, all constraints are honored.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public bool? IgnoreConstraints { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            this.ServiceFabricClient.Partitions.MoveAuxiliaryReplicaAsync(
                serviceId: this.ServiceId,
                partitionId: this.PartitionId,
                currentNodeName: this.CurrentNodeName,
                newNodeName: this.NewNodeName,
                ignoreConstraints: this.IgnoreConstraints,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
