// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the expected impact of a repair to a particular node.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class NodeImpact
    {
        /// <summary>
        /// Initializes a new instance of the NodeImpact class.
        /// </summary>
        /// <param name="nodeName">The name of the impacted node.</param>
        /// <param name="impactLevel">The level of impact expected. Possible values include: 'Invalid', 'None', 'Restart',
        /// 'RemoveData', 'RemoveNode'</param>
        public NodeImpact(
            string nodeName,
            ImpactLevel? impactLevel = default(ImpactLevel?))
        {
            nodeName.ThrowIfNull(nameof(nodeName));
            this.NodeName = nodeName;
            this.ImpactLevel = impactLevel;
        }

        /// <summary>
        /// Gets the name of the impacted node.
        /// </summary>
        public string NodeName { get; }

        /// <summary>
        /// Gets the level of impact expected. Possible values include: 'Invalid', 'None', 'Restart', 'RemoveData',
        /// 'RemoveNode'
        /// </summary>
        public ImpactLevel? ImpactLevel { get; }
    }
}
