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
    /// Gets ordered list of partitions.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFLoadedPartitionInfoList")]
    public partial class GetLoadedPartitionInfoListCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets MetricName. Name of the metric based on which to get ordered list of partitions.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public string MetricName { get; set; }

        /// <summary>
        /// Gets or sets ServiceName. The name of a service.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets Ordering. Ordering of partitions' load. Possible values include: 'Desc', 'Asc'
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, Position = 2)]
        public Ordering? Ordering { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, Position = 3)]
        public long? MaxResults { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            var continuationToken = default(ContinuationToken);
            do
            {
                var result = this.ServiceFabricClient.Partitions.GetLoadedPartitionInfoListAsync(
                    metricName: this.MetricName,
                    serviceName: this.ServiceName,
                    ordering: this.Ordering,
                    maxResults: this.MaxResults,
                    continuationToken: continuationToken,
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

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            return output;
        }
    }
}
