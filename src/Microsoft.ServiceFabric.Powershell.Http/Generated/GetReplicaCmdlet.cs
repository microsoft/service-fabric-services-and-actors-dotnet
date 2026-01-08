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
    /// Gets the information about replicas of a Service Fabric service partition.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFReplica", DefaultParameterSetName = "GetReplicaInfoList")]
    public partial class GetReplicaCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetReplicaInfoList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetReplicaInfo")]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets ReplicaId. The identifier of the replica.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetReplicaInfo")]
        public ReplicaId ReplicaId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetReplicaInfoList")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetReplicaInfo")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetReplicaInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Replicas.GetReplicaInfoListAsync(
                        partitionId: this.PartitionId,
                        continuationToken: continuationToken,
                        serverTimeout: this.ServerTimeout,
                        cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                    if (result == null)
                    {
                        break;
                    }

                    var count = 0;
                    foreach (var item in result.Data)
                    {
                        count++;
                        this.WriteObject(this.FormatOutput(item));
                    }

                    continuationToken = result.ContinuationToken;
                    this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
                }
                while (continuationToken.Next);
            }
            else if (this.ParameterSetName.Equals("GetReplicaInfo"))
            {
                var result = this.ServiceFabricClient.Replicas.GetReplicaInfoAsync(
                    partitionId: this.PartitionId,
                    replicaId: this.ReplicaId,
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
