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
    /// Gets the details of replica deployed on a Service Fabric node.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFDeployedServiceReplicaDetail", DefaultParameterSetName = "GetDeployedServiceReplicaDetailInfo")]
    public partial class GetDeployedServiceReplicaDetailCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedServiceReplicaDetailInfo")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetDeployedServiceReplicaDetailInfoByPartitionId")]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetDeployedServiceReplicaDetailInfo")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetDeployedServiceReplicaDetailInfoByPartitionId")]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets ReplicaId. The identifier of the replica.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetDeployedServiceReplicaDetailInfo")]
        public ReplicaId ReplicaId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedServiceReplicaDetailInfo")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetDeployedServiceReplicaDetailInfoByPartitionId")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetDeployedServiceReplicaDetailInfo"))
            {
                var result = this.ServiceFabricClient.Replicas.GetDeployedServiceReplicaDetailInfoAsync(
                    nodeName: this.NodeName,
                    partitionId: this.PartitionId,
                    replicaId: this.ReplicaId,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
            else if (this.ParameterSetName.Equals("GetDeployedServiceReplicaDetailInfoByPartitionId"))
            {
                var result = this.ServiceFabricClient.Replicas.GetDeployedServiceReplicaDetailInfoByPartitionIdAsync(
                    nodeName: this.NodeName,
                    partitionId: this.PartitionId,
                    serverTimeout: this.ServerTimeout,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    this.WriteObject(this.FormatOutput(result));
                }
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
