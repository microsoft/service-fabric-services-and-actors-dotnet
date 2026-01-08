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
    /// Gets the list of partitions of a Service Fabric service.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFPartition", DefaultParameterSetName = "GetPartitionInfoList")]
    public partial class GetPartitionCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ServiceId. The identity of the service. This ID is typically the full name of the service without the
        /// 'fabric:' URI scheme.
        /// Starting from version 6.0, hierarchical names are delimited with the "~" character.
        /// For example, if the service name is "fabric:/myapp/app1/svc1", the service identity would be "myapp~app1~svc1" in
        /// 6.0+ and "myapp/app1/svc1" in previous versions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetPartitionInfoList")]
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets PartitionId. The identity of the partition.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, ParameterSetName = "GetPartitionInfo")]
        public PartitionId PartitionId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetPartitionInfoList")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetPartitionInfo")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetPartitionInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Partitions.GetPartitionInfoListAsync(
                        serviceId: this.ServiceId,
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
            else if (this.ParameterSetName.Equals("GetPartitionInfo"))
            {
                var result = this.ServiceFabricClient.Partitions.GetPartitionInfoAsync(
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
            var outputResult = output as ServicePartitionInfo;

            var result = new PSObject();

            result.Properties.Add(new PSNoteProperty("PartitionId", outputResult.PartitionInformation.Id.ToString()));
            result.Properties.Add(new PSNoteProperty("PartitionKind", outputResult.PartitionInformation.ServicePartitionKind));

            if (outputResult.PartitionInformation is Int64RangePartitionInformation int64RangePartitionInformation)
            {
                result.Properties.Add(new PSNoteProperty("PartitionLowKey", int64RangePartitionInformation.LowKey));
                result.Properties.Add(new PSNoteProperty("PartitionHighKey", int64RangePartitionInformation.HighKey));
            }

            if (outputResult.PartitionInformation is NamedPartitionInformation namedPartitionInformation)
            {
                result.Properties.Add(new PSNoteProperty("PartitionName", namedPartitionInformation.Name));
            }

            if (output is StatefulServicePartitionInfo statefulServicePartitionInfo)
            {
                result.Properties.Add(new PSNoteProperty("DataLossNumber", statefulServicePartitionInfo.PrimaryEpoch.DataLossVersion));
                result.Properties.Add(new PSNoteProperty("ConfigurationNumber", statefulServicePartitionInfo.PrimaryEpoch.ConfigurationVersion));
            }

            return result;
        }
    }
}
