// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ImpactType.
    /// </summary>
    public enum ImpactType
    {
        /// <summary>
        /// The impact type is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The impact is related to a node deactivation.
        /// </summary>
        NodeDeactivation,

        /// <summary>
        /// The impact is related to an application upgrade.
        /// </summary>
        ApplicationUpgrade,

        /// <summary>
        /// The impact is related to a fabric upgrade.
        /// </summary>
        FabricUpgrade,

        /// <summary>
        /// The impact is related to a partition.
        /// </summary>
        Partition,
    }
}
