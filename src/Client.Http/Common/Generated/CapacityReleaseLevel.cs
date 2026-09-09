// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for CapacityReleaseLevel.
    /// </summary>
    public enum CapacityReleaseLevel
    {
        /// <summary>
        /// Restores the original target replica counts for primary, secondary, and auxiliary replicas.
        /// </summary>
        None,

        /// <summary>
        /// Releases capacity without making services unavailable. Services configured to drop replicas to zero retain their
        /// minimum replica set, services configured to drop replicas to their minimum retain their current primary and
        /// secondary targets, and auxiliary replica targets are reduced to zero.
        /// </summary>
        Minor,

        /// <summary>
        /// Releases additional capacity and can make services unavailable. Services configured to drop replicas to zero have
        /// their target replica count reduced to zero, services configured to drop replicas to their minimum have their target
        /// reduced to the minimum replica set size, and auxiliary replica targets are reduced to zero. Services reduced to
        /// zero become unavailable, and stateful services reduced to zero permanently lose their state.
        /// </summary>
        Major,
    }
}
