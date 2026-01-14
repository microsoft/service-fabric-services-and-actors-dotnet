// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item in NodeTypeHealthPolicyMap.
    /// </summary>
    public partial class NodeTypeHealthPolicyMapItem
    {
        /// <summary>
        /// Initializes a new instance of the NodeTypeHealthPolicyMapItem class.
        /// </summary>
        /// <param name="key">The key of the node type health policy map item. This is the name of the node type.</param>
        /// <param name="value">The value of the node type health policy map item.
        /// If the percentage is respected but there is at least one unhealthy node in the node type, the health is evaluated
        /// as Warning.
        /// The percentage is calculated by dividing the number of unhealthy nodes over the total number of nodes in the node
        /// type.
        /// The computation rounds up to tolerate one failure on small numbers of nodes.
        /// The max percent unhealthy nodes allowed for the node type. Must be between zero and 100.
        /// </param>
        public NodeTypeHealthPolicyMapItem(
            string key,
            int? value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key of the node type health policy map item. This is the name of the node type.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the value of the node type health policy map item.
        /// If the percentage is respected but there is at least one unhealthy node in the node type, the health is evaluated
        /// as Warning.
        /// The percentage is calculated by dividing the number of unhealthy nodes over the total number of nodes in the node
        /// type.
        /// The computation rounds up to tolerate one failure on small numbers of nodes.
        /// The max percent unhealthy nodes allowed for the node type. Must be between zero and 100.
        /// </summary>
        public int? Value { get; }
    }
}
