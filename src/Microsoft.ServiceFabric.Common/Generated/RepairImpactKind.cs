// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for RepairImpactKind.
    /// </summary>
    public enum RepairImpactKind
    {
        /// <summary>
        /// The repair impact is not valid or is of an unknown type.
        /// </summary>
        Invalid,

        /// <summary>
        /// The repair impact affects a set of Service Fabric nodes.
        /// </summary>
        Node,
    }
}
