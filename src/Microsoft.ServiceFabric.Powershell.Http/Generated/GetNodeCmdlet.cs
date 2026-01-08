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
    /// Gets the list of nodes in the Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFNode", DefaultParameterSetName = "GetNodeInfoList")]
    public partial class GetNodeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetNodeInfo")]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets NodeStatusFilter. Allows filtering the nodes based on the NodeStatus. Only the nodes that are matching
        /// the specified filter value will be returned. The filter value can be one of the following. Possible values include:
        /// 'default', 'all', 'up', 'down', 'enabling', 'disabling', 'disabled', 'unknown', 'removed'
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "GetNodeInfoList")]
        public NodeStatusFilter? NodeStatusFilter { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetNodeInfoList")]
        public long? MaxResults { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetNodeInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetNodeInfo")]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetNodeInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.Nodes.GetNodeInfoListAsync(
                        continuationToken: continuationToken,
                        nodeStatusFilter: this.NodeStatusFilter,
                        maxResults: this.MaxResults,
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
            else if (this.ParameterSetName.Equals("GetNodeInfo"))
            {
                var result = this.ServiceFabricClient.Nodes.GetNodeInfoAsync(
                    nodeName: this.NodeName,
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
