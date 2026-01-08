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
    /// Gets all Replicas-related events for a Partition.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFPartitionReplicasEvent", DefaultParameterSetName = "GetPartitionReplicasEventList")]
    public partial class GetPartitionReplicasEventCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetPartitionReplicaEventList")]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets StartTimeUtc. The start time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetPartitionReplicaEventList")]
        public string StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets EndTimeUtc. The end time of a lookup query in ISO UTC yyyy-MM-ddTHH:mm:ssZ.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, ParameterSetName = "GetPartitionReplicaEventList")]
        public string EndTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets ReplicaId. The identifier of the replica.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "GetPartitionReplicaEventList")]
        public ReplicaId ReplicaId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetPartitionReplicaEventList")]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets EventsTypesFilter. This is a comma separated string specifying the types of FabricEvents that should
        /// only be included in the response.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetPartitionReplicaEventList")]
        public string EventsTypesFilter { get; set; }

        /// <summary>
        /// Gets or sets ExcludeAnalysisEvents. This param disables the retrieval of AnalysisEvents if true is passed.
        /// </summary>
        [Parameter(Mandatory = false, Position = 6, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = false, Position = 6, ParameterSetName = "GetPartitionReplicaEventList")]
        public bool? ExcludeAnalysisEvents { get; set; }

        /// <summary>
        /// Gets or sets SkipCorrelationLookup. This param disables the search of CorrelatedEvents information if true is
        /// passed. otherwise the CorrelationEvents get processed and HasCorrelatedEvents field in every FabricEvent gets
        /// populated.
        /// </summary>
        [Parameter(Mandatory = false, Position = 7, ParameterSetName = "GetPartitionReplicasEventList")]
        [Parameter(Mandatory = false, Position = 7, ParameterSetName = "GetPartitionReplicaEventList")]
        public bool? SkipCorrelationLookup { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetPartitionReplicasEventList"))
            {
                var result = this.ServiceFabricClient.EventsStore.GetPartitionReplicasEventListAsync(
                    partitionId: this.PartitionId,
                    startTimeUtc: this.StartTimeUtc,
                    endTimeUtc: this.EndTimeUtc,
                    serverTimeout: this.ServerTimeout,
                    eventsTypesFilter: this.EventsTypesFilter,
                    excludeAnalysisEvents: this.ExcludeAnalysisEvents,
                    skipCorrelationLookup: this.SkipCorrelationLookup,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        this.WriteObject(this.FormatOutput(item));
                    }
                }
            }
            else if (this.ParameterSetName.Equals("GetPartitionReplicaEventList"))
            {
                var result = this.ServiceFabricClient.EventsStore.GetPartitionReplicaEventListAsync(
                    partitionId: this.PartitionId,
                    replicaId: this.ReplicaId,
                    startTimeUtc: this.StartTimeUtc,
                    endTimeUtc: this.EndTimeUtc,
                    serverTimeout: this.ServerTimeout,
                    eventsTypesFilter: this.EventsTypesFilter,
                    excludeAnalysisEvents: this.ExcludeAnalysisEvents,
                    skipCorrelationLookup: this.SkipCorrelationLookup,
                    cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                if (result != null)
                {
                    foreach (var item in result)
                    {
                        this.WriteObject(this.FormatOutput(item));
                    }
                }
            }
        }
    }
}
