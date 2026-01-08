// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ImpactLevel.
    /// </summary>
    public enum ImpactLevel
    {
        /// <summary>
        /// Invalid.
        /// </summary>
        Invalid,

        /// <summary>
        /// None.
        /// </summary>
        None,

        /// <summary>
        /// Restart.
        /// </summary>
        Restart,

        /// <summary>
        /// RemoveData.
        /// </summary>
        RemoveData,

        /// <summary>
        /// RemoveNode.
        /// </summary>
        RemoveNode,
    }
}
