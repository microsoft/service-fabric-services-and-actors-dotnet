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
    /// Resolve a Service Fabric partition.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Resolve, "SFService")]
    public partial class ResolveServiceCmdlet : CommonCmdletBase
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
        /// Gets or sets PartitionKeyType. Key type for the partition. This parameter is required if the partition scheme for
        /// the service is Int64Range or Named. The possible values are following.
        /// - None (1) - Indicates that the PartitionKeyValue parameter is not specified. This is valid for the partitions with
        /// partitioning scheme as Singleton. This is the default value. The value is 1.
        /// - Int64Range (2) - Indicates that the PartitionKeyValue parameter is an int64 partition key. This is valid for the
        /// partitions with partitioning scheme as Int64Range. The value is 2.
        /// - Named (3) - Indicates that the PartitionKeyValue parameter is a name of the partition. This is valid for the
        /// partitions with partitioning scheme as Named. The value is 3.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public int? PartitionKeyType { get; set; }

        /// <summary>
        /// Gets or sets PartitionKeyValue. Partition key. This is required if the partition scheme for the service is
        /// Int64Range or Named.
        /// This is not the partition ID, but rather, either the integer key value, or the name of the partition ID.
        /// For example, if your service is using ranged partitions from 0 to 10, then they PartitionKeyValue would be an
        /// integer in that range. Query service description to see the range or name.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string PartitionKeyValue { get; set; }

        /// <summary>
        /// Gets or sets PreviousRspVersion. The value in the Version field of the response that was received previously. This
        /// is required if the user knows that the result that was gotten previously is stale.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3)]
        public string PreviousRspVersion { get; set; }

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
            var result = this.ServiceFabricClient.Services.ResolveServiceAsync(
                serviceId: this.ServiceId,
                partitionKeyType: this.PartitionKeyType,
                partitionKeyValue: this.PartitionKeyValue,
                previousRspVersion: this.PreviousRspVersion,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            if (result != null)
            {
                this.WriteObject(this.FormatOutput(result));
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
