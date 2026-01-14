// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the parameters to restart a Service Fabric node.
    /// </summary>
    public partial class RestartNodeDescription
    {
        /// <summary>
        /// Initializes a new instance of the RestartNodeDescription class.
        /// </summary>
        /// <param name="nodeInstanceId">The instance ID of the target node. If instance ID is specified the node is restarted
        /// only if it matches with the current instance of the node. A default value of "0" would match any instance ID. The
        /// instance ID can be obtained using get node query.</param>
        /// <param name="createFabricDump">Specify True to create a dump of the fabric node process. This is case-sensitive.
        /// Possible values include: 'False', 'True'</param>
        public RestartNodeDescription(
            string nodeInstanceId = "0",
            CreateFabricDump? createFabricDump = Common.CreateFabricDump.False)
        {
            nodeInstanceId.ThrowIfNull(nameof(nodeInstanceId));
            this.NodeInstanceId = nodeInstanceId;
            this.CreateFabricDump = createFabricDump;
        }

        /// <summary>
        /// Gets the instance ID of the target node. If instance ID is specified the node is restarted only if it matches with
        /// the current instance of the node. A default value of "0" would match any instance ID. The instance ID can be
        /// obtained using get node query.
        /// </summary>
        public string NodeInstanceId { get; }

        /// <summary>
        /// Gets specify True to create a dump of the fabric node process. This is case-sensitive. Possible values include:
        /// 'False', 'True'
        /// </summary>
        public CreateFabricDump? CreateFabricDump { get; }
    }
}
