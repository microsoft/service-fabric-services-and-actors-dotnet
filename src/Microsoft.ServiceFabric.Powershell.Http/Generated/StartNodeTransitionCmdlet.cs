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
    /// Starts or stops a cluster node.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "SFNodeTransition")]
    public partial class StartNodeTransitionCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets NodeName. The name of the node.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        public NodeName NodeName { get; set; }

        /// <summary>
        /// Gets or sets OperationId. A GUID that identifies a call of this API.  This is passed into the corresponding
        /// GetProgress API
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public Guid? OperationId { get; set; }

        /// <summary>
        /// Gets or sets NodeTransitionType. Indicates the type of transition to perform.  NodeTransitionType.Start will start
        /// a stopped node.  NodeTransitionType.Stop will stop a node that is up. Possible values include: 'Invalid', 'Start',
        /// 'Stop'
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2)]
        public NodeTransitionType? NodeTransitionType { get; set; }

        /// <summary>
        /// Gets or sets NodeInstanceId. The node instance ID of the target node.  This can be determined through GetNodeInfo
        /// API.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 3)]
        public string NodeInstanceId { get; set; }

        /// <summary>
        /// Gets or sets StopDurationInSeconds. The duration, in seconds, to keep the node stopped.  The minimum value is 600,
        /// the maximum is 14400.  After this time expires, the node will automatically come back up.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 4)]
        public int? StopDurationInSeconds { get; set; }

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
            this.ServiceFabricClient.Faults.StartNodeTransitionAsync(
                nodeName: this.NodeName,
                operationId: this.OperationId,
                nodeTransitionType: this.NodeTransitionType,
                nodeInstanceId: this.NodeInstanceId,
                stopDurationInSeconds: this.StopDurationInSeconds,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
