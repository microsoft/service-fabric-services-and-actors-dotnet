// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ImpactOperationKind.
    /// </summary>
    public enum ImpactOperationKind
    {
        /// <summary>
        /// The operation kind is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// A restart operation on the impacted instance.
        /// </summary>
        Restart,

        /// <summary>
        /// A remove operation on the impacted instance.
        /// </summary>
        Remove,

        /// <summary>
        /// An add operation on the impacted instance.
        /// </summary>
        Add,
    }
}
