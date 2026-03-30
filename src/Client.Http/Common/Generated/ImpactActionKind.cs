// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ImpactActionKind.
    /// </summary>
    public enum ImpactActionKind
    {
        /// <summary>
        /// Reserved default value. Do not use.
        /// </summary>
        Unknown,

        /// <summary>
        /// Approves the impact, unblocking the associated operation.
        /// </summary>
        Approve,
    }
}
