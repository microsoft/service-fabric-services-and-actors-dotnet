// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the list of nodes targeted by a repair action.
    /// 
    /// This type supports the Service Fabric platform; it is not meant to be used directly from your code.
    /// </summary>
    public partial class NodeRepairTargetDescription : RepairTargetDescriptionBase
    {
        /// <summary>
        /// Initializes a new instance of the NodeRepairTargetDescription class.
        /// </summary>
        /// <param name="nodeNames">The list of nodes targeted by a repair action.</param>
        public NodeRepairTargetDescription(
            IEnumerable<string> nodeNames = default(IEnumerable<string>))
            : base(
                Common.RepairTargetKind.Node)
        {
            this.NodeNames = nodeNames;
        }

        /// <summary>
        /// Gets the list of nodes targeted by a repair action.
        /// </summary>
        public IEnumerable<string> NodeNames { get; }
    }
}
