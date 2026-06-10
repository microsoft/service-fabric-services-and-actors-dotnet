// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for ImpactApprovalStatus.
    /// </summary>
    public enum ImpactApprovalStatus
    {
        /// <summary>
        /// No approval status has been set.
        /// </summary>
        None,

        /// <summary>
        /// The impact is in a nominal state.
        /// </summary>
        Nominal,

        /// <summary>
        /// The impact is awaiting approval.
        /// </summary>
        WaitingForApproval,

        /// <summary>
        /// The impact has been approved.
        /// </summary>
        Approved,
    }
}
