// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the expected impact of a repair on a set of nodes.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class NodeRepairImpactDescription : RepairImpactDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the NodeRepairImpactDescription class.
        /// </summary>
        /// <param name="nodeImpactList">The list of nodes impacted by a repair action and their respective expected
        /// impact.</param>
        public NodeRepairImpactDescription(
            IEnumerable<NodeImpact> nodeImpactList = default(IEnumerable<NodeImpact>))
            : base(
                Common.RepairImpactKind.Node)
        {
            this.NodeImpactList = nodeImpactList;
        }

        /// <summary>
        /// Gets the list of nodes impacted by a repair action and their respective expected impact.
        /// </summary>
        public IEnumerable<NodeImpact> NodeImpactList { get; }
    }
}
